import React from "react";
import { withTranslation, Trans } from "react-i18next";
import styled, { css } from "styled-components";

import SnackBar from "@docspace/components/snackbar";

import Link from "@docspace/components/link";

const StyledLink = styled(Link)`
  font-size: 12px;
  line-height: 16px;
  font-weight: 400;

  color: #316daa;
`;

const QuotasBar = ({ t, type, onClick, onClose }) => {
  const quota = { header: "", description: "" };

  switch (type) {
    case "room":
      break;
    case "storage":
      break;
    case "confirm-email":
      quota.header = t("ConfirmEmailHeader");
      quota.description = (
        <>
          {t("ConfirmEmailDescription")}{" "}
          <StyledLink onClick={onClick}>{t("RequestActivation")}</StyledLink>
        </>
      );
      break;
    default:
      break;
  }

  const roomQuota = {
    header: t("RoomQuotaHeader", { currentValue: 7, maxValue: 12 }),
    description: (
      <Trans i18nKey="RoomQuotaDescription" t={t}>
        You can archived the unnecessary rooms or
        <StyledLink>{{ clickHere: t("ClickHere") }}</StyledLink> to find a
        better pricing plan for your portal.
      </Trans>
    ),
  };

  const storageQuota = {
    header: t("StorageQuotaQuotaHeader", { currentValue: 7, maxValue: 12 }),
    description: (
      <Trans i18nKey="StorageQuotaQuotaDescription" t={t}>
        You can remove the unnecessary files or
        <StyledLink onClick={onClick}>
          {{ clickHere: t("ClickHere") }}
        </StyledLink>{" "}
        to find a better pricing plan for your portal.
      </Trans>
    ),
  };

  return (
    <SnackBar
      headerText={quota.header}
      text={quota.description}
      isCampaigns={false}
      opacity={1}
      onLoad={() => console.log("load snackbar " + type)}
      clickAction={onClose}
    />
  );
};

export default withTranslation(["MainBar"])(QuotasBar);
