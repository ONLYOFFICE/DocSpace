const semver = require("semver");
const { fetchCommits } = require("./commits");

const MERGE_COMMIT_PATTERN = /^Merge (remote-tracking )?branch '.+'/;
const COMMIT_MESSAGE_PATTERN = /\n+([\S\s]+)/;

const parseReleases = async (tags, options, onParsed) => {
  const releases = await Promise.all(
    tags.map(async (tag) => {
      const commits = await fetchCommits(tag.diff, options);
      const merges = commits
        .filter((commit) => commit.merge)
        .map((commit) => commit.merge);
      const fixes = commits
        .filter((commit) => commit.fixes)
        .map((commit) => ({ fixes: commit.fixes, commit }));
      const emptyRelease = merges.length === 0 && fixes.length === 0;
      const { message } = commits[0] || { message: null };
      const breakingCount = commits.filter((c) => c.breaking).length;
      const filteredCommits = commits
        .filter(filterCommits(merges))
        .sort(sortCommits(options))
        .slice(0, getCommitLimit(options, emptyRelease, breakingCount));

      if (onParsed) onParsed(tag);

      const origins = commits.sort(sortCommits(options));

      return {
        ...tag,
        summary: getSummary(message, options),
        commits: filteredCommits,
        origins,
        groups: getGroups(commits),
        merges,
        fixes,
      };
    })
  );
  return releases.filter(filterReleases(options));
};

const getGroups = (commits) => {
  const grouped = commits.reduce((groups, commit) => {
    const niceDate = commit.niceDate;
    if (!groups[niceDate]) {
      groups[niceDate] = [];
    }
    if (!commit.merge && !MERGE_COMMIT_PATTERN.test(commit.subject))
      groups[niceDate].push(commit);

    return groups;
  }, {});

  // Edit: to add it in the array format instead
  const groupArrays = Object.keys(grouped)
    .map((niceDate) => {
      if (grouped[niceDate].length === 0) return null;

      const date = new Date(grouped[niceDate][0].date);

      return {
        niceDate,
        date,
        authors: getGroupedByAuthor(grouped[niceDate]),
      };
    })
    .filter((g) => g !== null && g.authors.length > 0);

  const sortedGroups = groupArrays
    .slice()
    .sort((a, b) => {
      return b.date - a.date;
    })
    .slice(0, 30); // last 30 days only

  // sortedGroups.forEach(({ niceDate }, i) =>
  //   console.log("niceDate", i, niceDate)
  // );

  return sortedGroups;
};

const getGroupedByAuthor = (commits) => {
  const grouped = commits.reduce((groups, commit) => {
    const { author } = commit;
    if (!groups[author]) {
      groups[author] = [];
    }
    groups[author].push(commit);

    return groups;
  }, {});

  const groupArrays = Object.keys(grouped).map((author) => {
    return {
      author,
      commits: grouped[author],
    };
  });

  return groupArrays;
};

const filterCommits = (merges) => (commit) => {
  if (commit.fixes || commit.merge) {
    // Filter out commits that already appear in fix or merge lists
    return false;
  }
  if (commit.breaking) {
    return true;
  }
  if (semver.valid(commit.subject)) {
    // Filter out version commits
    return false;
  }
  if (MERGE_COMMIT_PATTERN.test(commit.subject)) {
    // Filter out merge commits
    return false;
  }
  if (merges.findIndex((m) => m.message === commit.subject) !== -1) {
    // Filter out commits with the same message as an existing merge
    return false;
  }
  return true;
};

const sortCommits = ({ sortCommits }) => (a, b) => {
  if (!a.breaking && b.breaking) return 1;
  if (a.breaking && !b.breaking) return -1;
  if (sortCommits === "date") return new Date(a.date) - new Date(b.date);
  if (sortCommits === "date-desc") return new Date(b.date) - new Date(a.date);
  if (sortCommits === "subject") return a.subject.localeCompare(b.subject);
  if (sortCommits === "subject-desc") return b.subject.localeCompare(a.subject);
  return b.insertions + b.deletions - (a.insertions + a.deletions);
};

const getCommitLimit = (
  { commitLimit, backfillLimit },
  emptyRelease,
  breakingCount
) => {
  if (commitLimit === false) {
    return undefined; // Return all commits
  }
  const limit = emptyRelease ? backfillLimit : commitLimit;
  return Math.max(breakingCount, limit);
};

const getSummary = (message, { releaseSummary }) => {
  if (!message || !releaseSummary) {
    return null;
  }
  if (COMMIT_MESSAGE_PATTERN.test(message)) {
    return message.match(COMMIT_MESSAGE_PATTERN)[1];
  }
  return null;
};

const filterReleases = (options) => ({ merges, fixes, commits }) => {
  if (
    options.hideEmptyReleases &&
    merges.length + fixes.length + commits.length === 0
  ) {
    return false;
  }
  return true;
};

module.exports = {
  parseReleases,
};
