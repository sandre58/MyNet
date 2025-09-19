#!/usr/bin/env node
import { execSync } from 'child_process';
import { analyzeCommits } from '@semantic-release/commit-analyzer';
import pkg from 'semver';

const project = process.argv[2];
if (!project) {
  console.error("Usage: node calc-version.js <project>");
  process.exit(1);
}

// 1️⃣ Déterminer la branche et le tag courant
const branch = execSync('git rev-parse --abbrev-ref HEAD').toString().trim();
let currentTag = '';
try {
  currentTag = execSync('git describe --tags --exact-match').toString().trim();
} catch {
  currentTag = '';
}

// 2️⃣ Vérifier si on est sur un tag du projet
const isProjectTag = currentTag.startsWith(`${project}-v`);

// 3️⃣ Récupérer le dernier tag existant pour le projet
let lastTag = null;
try {
  lastTag = execSync(`git describe --tags --match "${project}-v*" --abbrev=0`).toString().trim();
} catch {
  lastTag = null;
}
let lastVersion = lastTag ? lastTag.replace(`${project}-v`, '') : '0.0.0';

// 4️⃣ Extraire les commits depuis le dernier tag (ou tout l’historique si aucun tag)
const fromRef = lastTag ? `${lastTag}..HEAD` : '';
const rawCommitsCmd = fromRef
  ? `git log ${fromRef} --pretty=format:%s -- src/${project}/`
  : `git log --pretty=format:%s -- src/${project}/`;
const rawCommits = execSync(rawCommitsCmd).toString().trim();

if (!rawCommits) {
  console.log(""); // pas de version
  process.exit(0);
}

const commits = rawCommits.split('\n').map(message => ({ message }));

(async () => {
  // 5️⃣ Déterminer le type de release à partir des commits
  const releaseType = await analyzeCommits({}, commits);
  if (!releaseType && !isProjectTag) {
    console.log(""); // pas de release
    process.exit(0);
  }

  // 6️⃣ Calcul de la version selon le contexte
  let nextVersion;

  if (isProjectTag) {
    // Release stable : on prend directement la version du tag
    nextVersion = currentTag.replace(`${project}-v`, '');
  } else if (branch === 'main') {
    // Sur main : prerelease (pre)
    nextVersion = pkg.inc(lastVersion, releaseType || 'patch', 'pre');
  } else {
    // Sur feature : prerelease (beta)
    nextVersion = pkg.inc(lastVersion, releaseType || 'patch', 'beta');
  }

  console.log(nextVersion);
})();