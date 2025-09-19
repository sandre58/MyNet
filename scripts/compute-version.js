#!/usr/bin/env node
import { execSync } from "child_process";
import { analyzeCommits } from "@semantic-release/commit-analyzer";
import semver from "semver";

const project = process.argv[2];
if (!project) {
  console.error("Usage: node calc-version.js <project>");
  process.exit(1);
}

// 1️⃣ Check if we are on a project tag
let currentTag = "";
try {
  currentTag = execSync("git describe --tags --exact-match").toString().trim();
} catch {
  currentTag = "";
}
if (currentTag.startsWith(`${project}/v`)) {
  const tagVersion = currentTag.replace(`${project}/v`, "");
  console.log(tagVersion);
  process.exit(0);
}

// 1️⃣ Get the latest existing tag for the project
let lastTag = null;
try {
  lastTag = execSync(`git describe --tags --match "${project}/v*" --abbrev=0`, {
    stdio: ["pipe", "pipe", "ignore"],
  })
    .toString()
    .trim();
} catch {
  lastTag = null;
}
let lastVersion = lastTag ? lastTag.replace(`${project}/v`, "") : "0.0.0";

// 3️⃣ Extract commits since last tag (or all history if no tag)
const fromRef = lastTag ? `${lastTag}..HEAD` : "";
const rawCommitsCmd = fromRef
  ? `git log ${fromRef} --pretty=format:%s -- src/${project}/`
  : `git log --pretty=format:%s -- src/${project}/`;
const rawCommits = execSync(rawCommitsCmd).toString().trim();

if (!rawCommits) {
  console.log(lastVersion); // no version
  process.exit(0);
}

const commits = rawCommits
  .split("\n")
  .map((message, index) => ({
    message: message.trim(),
    hash: `commit${index}`,
    subject: message.trim(),
  }))
  .filter((commit) => commit.message);

(async () => {
  // 4️⃣ Determine release type from commits
  const releaseType = await analyzeCommits(
    { preset: "conventionalcommits" }, // ou 'conventionalcommits' si installé
    {
      commits,
      logger: {
        log: console.error,
        error: console.error,
      },
    }
  );

  if (!releaseType) {
    console.log(lastVersion);
    process.exit(0);
  }

  // 5️⃣ Calculate version based on context
  const branch = execSync("git rev-parse --abbrev-ref HEAD").toString().trim();
  let suffix;
  if (branch === "main") {
    suffix = "pre";
  } else {
    suffix = "beta";
  }
  let nextVersion = semver.inc(lastVersion, releaseType || "patch", suffix);

  console.log(nextVersion);
})();
