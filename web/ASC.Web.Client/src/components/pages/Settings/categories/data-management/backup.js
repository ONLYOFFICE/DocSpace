import React, { Suspense, lazy, useState } from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import { useTranslation } from "react-i18next";
import ArrowRightIcon from "../../../../../../public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import styled from "styled-components";

const StyledComponent = styled.div`
  .category-item-wrapper {
    margin-bottom: 40px;

    .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 5px;
    }

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: #555f65;
      font-size: 12px;
      max-width: 1024px;
    }

    .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    }

    .link-text {
      margin: 0;
    }
  }
`;
const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;
const Backup = ({ history }) => {
  const onClickLink = (e) => {
    e.preventDefault();

    console.log("history", history);
    history.push(e.target.pathname);
  };

  const { t } = useTranslation("Settings");
  return (
    <StyledComponent>
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/settings/datamanagement/backup/automatic-backup"
            )}
          >
            {t("AutomaticBackup")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("AutomaticBackupSettingsDescription")}
        </Text>
      </div>

      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/settings/datamanagement/backup/manual-backup"
            )}
          >
            {t("ManualBackup")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("ManualBackupSettingsDescription")}
        </Text>
        <Text className="category-item-description">
          {t("ManualBackupSettingsWarningDescription")}
        </Text>
      </div>
    </StyledComponent>
  );
};

export default Backup;
