import React from "react";
import { useLocation } from "react-router-dom";
import { useTranslation } from "react-i18next";

import Headline from "@docspace/common/components/Headline";

import { getItemByLink } from "SRC_DIR/utils";

const SectionHeaderContent = () => {
  const { t } = useTranslation(["Settings", "Common"]);

  const location = useLocation();
  const path = location.pathname;
  const item = getItemByLink(path);

  return (
    <Headline type="content" truncate={true}>
      <div className="header">{t(item?.tKey)}</div>
    </Headline>
  );
};

export default SectionHeaderContent;
