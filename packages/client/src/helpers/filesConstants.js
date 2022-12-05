export const GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
export const TIMEOUT = 1000;
export const EDITOR_PROTOCOL = "oo-office";

export const thumbnailStatuses = {
  WAITING: 0,
  CREATED: 1,
  ERROR: 2,
  NOT_REQUIRED: 3,
};

export const ADS_TIMEOUT = 300000; // 5 min

export const FilterGroups = Object.freeze({
  filterType: "filter-filterType",
  filterAuthor: "filter-author",
  filterFolders: "filter-folders",
  filterContent: "filter-withContent",
  roomFilterProviderType: "filter-provider-type",
  roomFilterType: "filter-type",
  roomFilterSubject: "filter-subject",
  roomFilterOwner: "filter-owner",
  roomFilterTags: "filter-tags",
  roomFilterFolders: "filter-withSubfolders",
  roomFilterContent: "filter-content",
});

export const FilterKeys = Object.freeze({
  withSubfolders: "withSubfolders",
  excludeSubfolders: "excludeSubfolders",
  withContent: "withContent",
  me: "me",
  other: "other",
  user: "user",
});
