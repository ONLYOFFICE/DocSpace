import React from "react";
import styled from "styled-components";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { getMinifyTitle } from "SRC_DIR/utils";
import { useStore } from "SRC_DIR/store";

const StyledHeader = styled.h1`
  padding: 12px 4px;
  font-size: 17px;
  font-style: normal;
  font-weight: 700;
  line-height: 22px;
`;

const ArticleHeaderContent = () => {
  const { t } = useTranslation("Common");
  const { authStore } = useStore();
  const { settingsStore } = authStore;
  const { showText } = settingsStore;

  const title = !showText
    ? getMinifyTitle(t("SpaceManagement"))
    : t("SpaceManagement");

  return <StyledHeader>{title}</StyledHeader>;
};

export default observer(ArticleHeaderContent);
