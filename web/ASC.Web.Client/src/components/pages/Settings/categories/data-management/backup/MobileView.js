import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import ArrowRightIcon from "../../../../../../../public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import Box from "@appserver/components/box";
import styled from "styled-components";
import FloatingButton from "@appserver/common/components/FloatingButton";
import { enableAutoBackup, enableRestore } from "@appserver/common/api/portal";
import Loader from "@appserver/components/loader";
import { StyledBackup } from "./StyledBackup";

const { proxyURL } = AppServerConfig;

import moment from "moment";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

class BackupMobileView extends React.Component {
  constructor(props) {
    super(props);
    const { language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this.state = {
      enableRestore: false,
      enableAutoBackup: false,
      isLoading: true,
    };
  }

  componentDidMount() {
    this.setBasicSettings();
  }
  componentWillUnmount() {
    const { clearProgressInterval } = this.props;

    clearProgressInterval();
  }
  setBasicSettings = async () => {
    const { t, getProgress } = this.props;

    const requests = [enableRestore(), enableAutoBackup()];

    try {
      getProgress(t);

      const [restore, autoBackup] = await Promise.allSettled(requests);

      const canRestore = restore.value;
      const canAutoBackup = autoBackup.value;

      this.setState({
        isLoading: false,
        enableRestore: canRestore,
        enableAutoBackup: canAutoBackup,
      });
    } catch (error) {
      console.error(error);
      this.setState({
        isLoading: false,
      });
    }
  };

  onClickLink = (e) => {
    const { history } = this.props;
    e.preventDefault();
    history.push(e.target.pathname);
  };

  onClickFloatingButton = () => {
    const { history } = this.props;
    history.push(
      combineUrl(proxyURL, "/settings/datamanagement/backup/manual-backup")
    );
  };
  render() {
    const { t, helpUrlCreatingBackup, downloadingProgress } = this.props;
    const { isLoading, enableRestore, enableAutoBackup } = this.state;

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBackup>
        {enableAutoBackup && (
          <div className="category-item-wrapper">
            <div className="category-item-heading">
              <Link
                truncate={true}
                className="inherit-title-link header"
                onClick={this.onClickLink}
                href={combineUrl(
                  AppServerConfig.proxyURL,
                  "/settings/datamanagement/backup/automatic-backup"
                )}
              >
                {t("AutoBackup")}
              </Link>
              <StyledArrowRightIcon size="small" color="#333333" />
            </div>

            <Text className="category-item-description">
              {t("AutomaticBackupSettingsDescription")}
            </Text>
          </div>
        )}

        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              truncate={true}
              className="inherit-title-link header"
              onClick={this.onClickLink}
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
            {t("ManualBackupSettingsNoteDescription")}
          </Text>
        </div>

        {enableRestore && (
          <div className="category-item-wrapper">
            <div className="category-item-heading">
              <Link
                truncate={true}
                className="inherit-title-link header"
                onClick={this.onClickLink}
                href={combineUrl(
                  AppServerConfig.proxyURL,
                  "/settings/datamanagement/backup/restore-backup"
                )}
              >
                {t("RestoreBackup")}
              </Link>
              <StyledArrowRightIcon size="small" color="#333333" />
            </div>
            <Text className="category-item-description">
              {t("RestoreBackupSettingsDescription")}
            </Text>
            <Box marginProp="16px 0 0 0">
              <Link
                color="#316DAA"
                target="_blank"
                isHovered={true}
                href={helpUrlCreatingBackup}
              >
                {t("Common:LearnMore")}
              </Link>
            </Box>
          </div>
        )}

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
            onClick={this.onClickFloatingButton}
          />
        )}
      </StyledBackup>
    );
  }
}

export default inject(({ auth, backup }) => {
  const { language } = auth;
  const { helpUrlCreatingBackup } = auth.settingsStore;
  const { getProgress, downloadingProgress, clearProgressInterval } = backup;
  return {
    helpUrlCreatingBackup,
    language,
    getProgress,
    downloadingProgress,
    clearProgressInterval,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupMobileView)));
