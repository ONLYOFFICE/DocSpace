import React from "react";
import { withTranslation, Trans } from "react-i18next";
import styled, { css } from "styled-components";

import SnackBar from "@docspace/components/snackbar";

import Link from "@docspace/components/link";
import { QuotaBarTypes } from "SRC_DIR/helpers/constants";

const StyledLink = styled(Link)`
  font-size: 12px;
  line-height: 16px;
  font-weight: 400;

  color: ${(props) => props.currentColorScheme.main.accent};
`;

const QuotasBar = ({
  t,
  tReady,
  type,
  currentValue,
  maxValue,
  onClick,
  onClose,
  onLoad,
  currentColorScheme,
}) => {
  const onClickAction = () => {
    onClick && onClick(type);
  };

  const onCloseAction = () => {
    onClose && onClose(type);
  };

  const getQuotaInfo = () => {
    switch (type) {
      case QuotaBarTypes.RoomQuota:
        return {
          header: t("RoomQuotaHeader", { currentValue, maxValue }),
          description: (
            <Trans i18nKey="RoomQuotaDescription" t={t}>
              You can archived the unnecessary rooms or
              <StyledLink
                currentColorScheme={currentColorScheme}
                onClick={onClickAction}
              >
                {{ clickHere: t("ClickHere").toLowerCase() }}
              </StyledLink>{" "}
              to find a better pricing plan for your portal.
            </Trans>
          ),
        };
      case QuotaBarTypes.StorageQuota:
        return {
          header: t("StorageQuotaHeader", { currentValue, maxValue }),
          description: (
            <Trans i18nKey="StorageQuotaDescription" t={t}>
              You can remove the unnecessary files or
              <StyledLink
                currentColorScheme={currentColorScheme}
                onClick={onClickAction}
              >
                {{ clickHere: t("ClickHere").toLowerCase() }}
              </StyledLink>{" "}
              to find a better pricing plan for your portal.
            </Trans>
          ),
        };
      case QuotaBarTypes.UserQuota:
        return {
          header: t("UserQuotaHeader", { currentValue, maxValue }),
          description: (
            <Trans i18nKey="UserQuotaDescription" t={t}>
              {""}
              <StyledLink
                currentColorScheme={currentColorScheme}
                onClick={onClickAction}
              >
                {{ clickHere: t("ClickHere") }}
              </StyledLink>{" "}
              to find a better pricing plan for your portal.
            </Trans>
          ),
        };
      case QuotaBarTypes.UserAndStorageQuota:
        return {
          header: t("StorageAndUserHeader", { currentValue, maxValue }),
          description: (
            <Trans i18nKey="UserQuotaDescription" t={t}>
              {""}
              <StyledLink
                currentColorScheme={currentColorScheme}
                onClick={onClickAction}
              >
                {{ clickHere: t("ClickHere") }}
              </StyledLink>{" "}
              to find a better pricing plan for your portal.
            </Trans>
          ),
        };
      case QuotaBarTypes.RoomAndStorageQuota:
        return {
          header: t("StorageAndRoomHeader", { currentValue, maxValue }),
          description: (
            <Trans i18nKey="UserQuotaDescription" t={t}>
              {""}
              <StyledLink
                currentColorScheme={currentColorScheme}
                onClick={onClickAction}
              >
                {{ clickHere: t("ClickHere") }}
              </StyledLink>{" "}
              to find a better pricing plan for your portal.
            </Trans>
          ),
        };

      default:
        return null;
    }
  };

  const quotaInfo = getQuotaInfo();

  return tReady && quotaInfo ? (
    <SnackBar
      headerText={quotaInfo.header}
      text={quotaInfo.description}
      isCampaigns={false}
      opacity={1}
      onLoad={onLoad}
      clickAction={onCloseAction}
    />
  ) : (
    <></>
  );
};

export default withTranslation(["MainBar"])(QuotasBar);
