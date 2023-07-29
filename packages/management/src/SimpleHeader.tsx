import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

const StyledHeader = styled.div`
  position: absolute;
  right: 50%;
  transform: translateX(50%);
  font-size: 17px;
  font-style: normal;
  font-weight: 700;
  line-height: 22px;
`;

const SimpleHeader = () => {
  const { t } = useTranslation("Common");

  return <StyledHeader>{t("SpaceManagement")}</StyledHeader>;
};

export default SimpleHeader;
