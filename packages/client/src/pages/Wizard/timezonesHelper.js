import { findWindows } from "windows-iana";

export const getUserTimezone = () => {
  return Intl.DateTimeFormat().resolvedOptions().timeZone || "UTC";
};

export const getSelectZone = (zones, userTimezone) => {
  const defaultTimezone = "UTC";
  const isWindowsZones = zones[0].key === "Dateline Standard Time"; //TODO: get from server

  if (isWindowsZones) {
    const windowsZoneKey = findWindows(userTimezone);
    return (
      zones.filter((zone) => zone.key === windowsZoneKey[0]) ||
      zones.filter((zone) => zone.key === defaultTimezone)
    );
  }
  return (
    zones.filter((zone) => zone.key === userTimezone) ||
    zones.filter((zone) => zone.key === defaultTimezone)
  );
};
