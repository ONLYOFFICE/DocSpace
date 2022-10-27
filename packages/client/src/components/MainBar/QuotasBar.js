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

const QuotasBar = ({
  t,
  isRoomQuota,
  currentValue,
  maxValue,
  onClick,
  onClose,
  onLoad,
}) => {
  const onClickAction = () => {
    onClick && onClick(isRoomQuota);
  };

  const onCloseAction = () => {
    onClose && onClose(isRoomQuota);
  };

  const roomQuota = {
    header: t("RoomQuotaHeader", { currentValue, maxValue }),
    description: (
      <Trans i18nKey="RoomQuotaDescription" t={t}>
        You can archived the unnecessary rooms or
        <StyledLink onClick={onClickAction}>
          {{ clickHere: t("ClickHere") }}
        </StyledLink>{" "}
        to find a better pricing plan for your portal.
      </Trans>
    ),
  };

  const storageQuota = {
    header: t("StorageQuotaHeader", { currentValue, maxValue }),
    description: (
      <Trans i18nKey="StorageQuotaDescription" t={t}>
        You can remove the unnecessary files or{" "}
        <StyledLink onClick={onClickAction}>
          {{ clickHere: t("ClickHere") }}
        </StyledLink>{" "}
        to find a better pricing plan for your portal.
      </Trans>
    ),
  };

  return isRoomQuota ? (
    <SnackBar
      headerText={roomQuota.header}
      text={roomQuota.description}
      isCampaigns={false}
      opacity={1}
      onLoad={onLoad}
      clickAction={onCloseAction}
    />
  ) : (
    <SnackBar
      headerText={storageQuota.header}
      text={storageQuota.description}
      isCampaigns={false}
      opacity={1}
      onLoad={onLoad}
      clickAction={onCloseAction}
    />
  );
};

export default withTranslation(["MainBar"])(QuotasBar);
