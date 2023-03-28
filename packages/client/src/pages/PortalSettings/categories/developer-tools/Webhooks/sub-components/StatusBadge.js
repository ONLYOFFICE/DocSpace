import React, { useState, useEffect } from "react";
import { Badge } from "@docspace/components";

export const StatusBadge = ({ status }) => {
  const [badgeColorScheme, setBadgeColorScheme] = useState({
    backgroundColor: "#2DB4821A",
    color: "#2DB482",
  });

  useEffect(() => {
    if (status < 200 || status > 299) {
      setBadgeColorScheme({
        backgroundColor: "#F21C0E1A",
        color: "#F21C0E",
      });
    }
  }, []);

  return (
    <Badge
      backgroundColor={badgeColorScheme.backgroundColor}
      color={badgeColorScheme.color}
      label={status}
      fontSize="9px"
      fontWeight={700}
      noHover
    />
  );
};
