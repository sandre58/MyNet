#!/usr/bin/env node
import { execSync } from 'child_process';
import { analyzeCommits } from '@semantic-release/commit-analyzer';
import semver from 'semver';

const project = process.argv[2];
if (!project) {
  console.error("Usage: node calc-version.js <project>");
  process.exit(1);
}

// 1️⃣ Determine current branch and tag
const branch = execSync('git rev-parse --abbrev-ref HEAD').toString().trim();
let currentTag = '';
try {
  currentTag = execSync('git describe --tags --exact-match', { stdio: ['pipe', 'pipe', 'ignore'] }).toString().trim();
} catch {
  currentTag = '';
}

// 2️⃣ Check if we are on a project tag
const isProjectTag = currentTag.startsWith(`${project}-v`);

// 3️⃣ Get the latest existing tag for the project
let lastTag = null;
try {
  lastTag = execSync(`git describe --tags --match "${project}-v*" --abbrev=0`).toString().trim();
} catch {
  lastTag = null;
}
let lastVersion = lastTag ? lastTag.replace(`${project}-v`, '') : '0.0.0';

// 4️⃣ Extract commits since last tag (or all history if no tag)
const fromRef = lastTag ? `${lastTag}..HEAD` : '';
const rawCommitsCmd = fromRef
  ? `git log ${fromRef} --pretty=format:%s -- src/${project}/`
  : `git log --pretty=format:%s -- src/${project}/`;
const rawCommits = execSync(rawCommitsCmd).toString().trim();

if (!rawCommits) {
  console.log(""); // no version
  process.exit(0);
}

const commits = rawCommits.split('\n').map((message, index) => ({ 
  message: message.trim(),
  hash: `commit${index}`,
  subject: message.trim()
})).filter(commit => commit.message);

(async () => {
  // 5️⃣ Determine release type from commits
  const releaseType = await analyzeCommits({ preset: 'conventionalcommits' }, { commits });
  if (!releaseType && !isProjectTag) {
    console.log(""); // no release
    process.exit(0);
  }

  // 6️⃣ Calculate version based on context
  let nextVersion;

  if (isProjectTag) {
    // Stable release: take version directly from tag
    nextVersion = currentTag.replace(`${project}-v`, '');
  } else if (branch === 'main') {
    // On main: prerelease (pre)
    nextVersion = semver.inc(lastVersion, releaseType || 'patch', 'pre');
  } else {
    // On feature: prerelease (beta)
    nextVersion = semver.inc(lastVersion, releaseType || 'patch', 'beta');
  }

  console.log(nextVersion);
})();