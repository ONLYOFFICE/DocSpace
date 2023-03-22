import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import SnackBar from "@docspace/components/snackbar";

import Link from "@docspace/components/link";

const ConfirmEmailBar = ({
  t,
  tReady,
  onClick,
  onClose,
  onLoad,
  currentColorScheme,
}) => {
  return (
    tReady && (
      <SnackBar
        headerText={t("ConfirmEmailHeader")}
        text={
          <>
            {t("ConfirmEmailDescription")}{" "}
            <Link
              fontSize="12px"
              fontWeight="400"
              color={currentColorScheme?.main?.accent}
              onClick={onClick}
            >
              {t("RequestActivation")}
            </Link>
          </>
        }
        isCampaigns={false}
        opacity={1}
        onLoad={onLoad}
        clickAction={onClose}
      />
    )
  );
};

export default withTranslation(["MainBar"])(ConfirmEmailBar);
