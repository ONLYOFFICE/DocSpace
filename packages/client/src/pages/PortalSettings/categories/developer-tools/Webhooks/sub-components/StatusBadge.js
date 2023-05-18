import React, { useState, useEffect } from "react";
import { Badge } from "@docspace/components";

import { useTranslation } from "react-i18next";

export const StatusBadge = ({ status }) => {
  const [badgeColorScheme, setBadgeColorScheme] = useState({
    backgroundColor: "#2DB4821A",
    color: "#2DB482",
  });
  const { t } = useTranslation(["Webhooks"]);

  useEffect(() => {
    if (status < 200 || status > 299) {
      setBadgeColorScheme({
        backgroundColor: "#F21C0E1A",
        color: "#F21C0E",
      });
    }
  }, []);
  if (status === undefined) {
    return;
  }

  return (
    <Badge
      backgroundColor={badgeColorScheme.backgroundColor}
      color={badgeColorScheme.color}
      label={status === 0 ? t("NotSent", { ns: "Webhooks" }) : status.toString()}
      fontSize="9px"
      fontWeight={700}
      noHover
    />
  );
};
