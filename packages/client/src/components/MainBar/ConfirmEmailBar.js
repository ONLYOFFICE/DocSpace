import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import SnackBar from "@docspace/components/snackbar";

import Link from "@docspace/components/link";

const StyledLink = styled(Link)`
  font-size: 12px;
  line-height: 16px;
  font-weight: 400;

  color: #316daa;
`;

const ConfirmEmailBar = ({ t, onClick, onClose, onLoad }) => {
  return (
    <SnackBar
      headerText={t("ConfirmEmailHeader")}
      text={
        <>
          {t("ConfirmEmailDescription")}{" "}
          <StyledLink onClick={onClick}>{t("RequestActivation")}</StyledLink>
        </>
      }
      isCampaigns={false}
      opacity={1}
      onLoad={onLoad}
      clickAction={onClose}
    />
  );
};

export default withTranslation(["MainBar"])(ConfirmEmailBar);
