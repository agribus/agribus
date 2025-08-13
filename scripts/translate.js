import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const DEEPL_API_KEY = "";
const INPUT_FILE = path.join(__dirname, "../apps/lychee_client/public/i18n/fr.json");

const delay = ms => new Promise(resolve => setTimeout(resolve, ms));

async function checkDeepLUsage() {
  try {
    const params = new URLSearchParams();
    params.append("auth_key", DEEPL_API_KEY);

    const res = await fetch("https://api-free.deepl.com/v2/usage", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: params,
    });

    const bodyText = await res.text();

    if (!res.ok) {
      console.error(`❌ Error checking usage: ${res.status} ${res.statusText}`, bodyText);
      return;
    }

    let data;
    try {
      data = JSON.parse(bodyText);
    } catch (err) {
      console.error("❌ Error parsing usage response:", bodyText);
      return;
    }

    const { character_count, character_limit } = data;
    const usagePercentage = ((character_count / character_limit) * 100).toFixed(2);
    const remainingChars = character_limit - character_count;

    console.log("📊 DeepL API Usage Summary:");
    console.log(`   Characters used: ${character_count.toLocaleString()}`);
    console.log(`   Character limit: ${character_limit.toLocaleString()}`);
    console.log(`   Usage: ${usagePercentage}%`);
    console.log(`   Remaining: ${remainingChars.toLocaleString()} characters`);

    if (usagePercentage > 90) {
      console.log("⚠️  Warning: You're approaching your character limit!");
    } else if (usagePercentage > 75) {
      console.log("💡 Info: You've used more than 75% of your character limit");
    }
  } catch (error) {
    console.error("❌ Error checking DeepL usage:", error.message);
  }
}

function getTranslationsDiff(sourceData, existingTranslationsPath) {
  let existingData = {};

  // Load existing translations if file exists
  if (fs.existsSync(existingTranslationsPath)) {
    try {
      existingData = JSON.parse(fs.readFileSync(existingTranslationsPath, "utf8"));
      console.log(
        `📄 Loaded existing translations from: ${path.basename(existingTranslationsPath)}`
      );
    } catch (err) {
      console.warn(`⚠️ Could not read existing translations: ${err.message}`);
      existingData = {};
    }
  } else {
    console.log(
      `📄 No existing translation file found at: ${path.basename(existingTranslationsPath)}`
    );
  }

  // Find missing translations
  function findMissing(source, existing, result = {}) {
    for (const key in source) {
      if (typeof source[key] === "string") {
        if (!existing[key] || existing[key] === source[key]) {
          result[key] = source[key];
        }
      } else if (typeof source[key] === "object" && source[key] !== null) {
        if (!existing[key]) {
          result[key] = source[key];
        } else {
          const nestedMissing = findMissing(source[key], existing[key], {});
          if (Object.keys(nestedMissing).length > 0) {
            result[key] = nestedMissing;
          }
        }
      }
    }
    return result;
  }

  const missingTranslations = findMissing(sourceData, existingData);

  return {
    existing: existingData,
    missing: missingTranslations,
    hasMissing: Object.keys(missingTranslations).length > 0,
  };
}

function mergeTranslations(existing, newTranslations) {
  function deepMerge(target, source) {
    for (const key in source) {
      if (typeof source[key] === "object" && source[key] !== null) {
        if (!target[key] || typeof target[key] !== "object") {
          target[key] = {};
        }
        deepMerge(target[key], source[key]);
      } else {
        target[key] = source[key];
      }
    }
    return target;
  }

  return deepMerge({ ...existing }, newTranslations);
}

function countStringsInObject(obj) {
  let count = 0;
  const traverse = o => {
    if (typeof o === "string") {
      count++;
    } else if (typeof o === "object" && o !== null) {
      Object.values(o).forEach(traverse);
    }
  };
  traverse(obj);
  return count;
}

async function translateText(text, targetLang, retries = 3) {
  if (!text || typeof text !== "string" || text.trim() === "") {
    return text;
  }

  for (let attempt = 1; attempt <= retries; attempt++) {
    try {
      const params = new URLSearchParams();
      params.append("auth_key", DEEPL_API_KEY);
      params.append("text", text);
      params.append("target_lang", targetLang);
      params.append("source_lang", "FR");

      const res = await fetch("https://api-free.deepl.com/v2/translate", {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: params,
      });

      const bodyText = await res.text();

      if (!res.ok) {
        console.error(`HTTP Error ${res.status} ${res.statusText}`, bodyText);

        if (res.status === 429) {
          console.log(`Rate limit reached, waiting ${attempt * 2} seconds...`);
          await delay(attempt * 2000);
          continue;
        }

        throw new Error(`DeepL Error: ${res.status} ${res.statusText}`);
      }

      let data;
      try {
        data = JSON.parse(bodyText);
      } catch (err) {
        console.error("JSON parsing error, received body:", bodyText);
        throw err;
      }

      const translation = data.translations?.[0]?.text || text;
      console.log(`✓ "${text}" -> "${translation}" (${targetLang})`);

      await delay(100);

      return translation;
    } catch (error) {
      console.error(`Attempt ${attempt}/${retries} failed for "${text}":`, error.message);

      if (attempt === retries) {
        console.error(`Final failure for "${text}", keeping original text`);
        return text;
      }

      await delay(1000 * attempt);
    }
  }
}

async function translateObject(obj, targetLang, currentPath = "") {
  if (typeof obj === "string") {
    return await translateText(obj, targetLang);
  } else if (Array.isArray(obj)) {
    const results = [];
    for (let i = 0; i < obj.length; i++) {
      console.log(`Translating array element ${i} of ${currentPath}...`);
      results[i] = await translateObject(obj[i], targetLang, `${currentPath}[${i}]`);
    }
    return results;
  } else if (typeof obj === "object" && obj !== null) {
    const result = {};
    const keys = Object.keys(obj);

    for (let i = 0; i < keys.length; i++) {
      const key = keys[i];
      const newPath = currentPath ? `${currentPath}.${key}` : key;
      console.log(`Translating ${newPath}... (${i + 1}/${keys.length})`);
      result[key] = await translateObject(obj[key], targetLang, newPath);
    }
    return result;
  }
  return obj;
}

async function processLanguage(sourceData, targetLang, outputPath, languageName, flag) {
  console.log(`\n${flag} Processing ${languageName} translation...`);

  let existingData = {};
  if (fs.existsSync(outputPath)) {
    existingData = JSON.parse(fs.readFileSync(outputPath, "utf8"));
  }

  const completeTranslations = await buildFullTranslation(sourceData, existingData, targetLang);

  fs.writeFileSync(outputPath, JSON.stringify(completeTranslations, null, 2), "utf8");
  console.log(`✅ ${languageName} translation updated: ${outputPath}`);
  console.log(`💾 Total translations: ${countStringsInObject(completeTranslations)}`);
}

async function buildFullTranslation(sourceObj, existingObj, targetLang, currentPath = "") {
  if (typeof sourceObj === "string") {
    if (existingObj && existingObj !== sourceObj) return existingObj;
    return await translateText(sourceObj, targetLang);
  }

  if (Array.isArray(sourceObj)) {
    const arr = [];
    for (let i = 0; i < sourceObj.length; i++) {
      arr[i] = await buildFullTranslation(
        sourceObj[i],
        existingObj?.[i],
        targetLang,
        `${currentPath}[${i}]`
      );
    }
    return arr;
  }

  if (typeof sourceObj === "object" && sourceObj !== null) {
    const result = {};
    for (const key of Object.keys(sourceObj)) {
      result[key] = await buildFullTranslation(
        sourceObj[key],
        existingObj?.[key],
        targetLang,
        currentPath ? `${currentPath}.${key}` : key
      );
    }
    return result;
  }

  return sourceObj;
}

async function main() {
  try {
    if (!fs.existsSync(INPUT_FILE)) {
      throw new Error(`Source file does not exist: ${INPUT_FILE}`);
    }

    console.log(`📖 Reading source file: ${INPUT_FILE}`);
    const frData = JSON.parse(fs.readFileSync(INPUT_FILE, "utf8"));
    const totalSourceStrings = countStringsInObject(frData);
    console.log(`📊 Total source strings: ${totalSourceStrings}`);

    const outputDir = path.join(__dirname, "../apps/lychee_client/public/i18n");

    // Process English translations
    const enFilePath = path.join(outputDir, "en.json");
    await processLanguage(frData, "EN", enFilePath, "English", "🇬🇧");

    // Pause between languages
    console.log("\n⏸️ 2-second pause between languages...");
    await delay(2000);

    // Process German translations
    const deFilePath = path.join(outputDir, "de.json");
    await processLanguage(frData, "DE", deFilePath, "German", "🇩🇪");

    console.log("\n🎉 All translations completed successfully!");

    console.log("\n📈 Checking DeepL API usage...");
    await checkDeepLUsage();
  } catch (error) {
    console.error("\n❌ Error during execution:", error.message);
    console.error("Stack trace:", error.stack);
    process.exit(1);
  }
}

process.on("SIGINT", () => {
  console.log("\n⚠️ Interruption detected. Stopping script...");
  process.exit(0);
});

process.on("SIGTERM", () => {
  console.log("\n⚠️ Termination detected. Stopping script...");
  process.exit(0);
});

main();
