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
import {
  enableAutoBackup,
  enableRestore,
  getBackupProgress,
  getBackupSchedule,
} from "@appserver/common/api/portal";
import Loader from "@appserver/components/loader";
import { StyledBackup } from "./StyledBackup";

const { proxyURL } = AppServerConfig;

import toastr from "@appserver/components/toast/toastr";
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
      downloadingProgress: 100,
      enableRestore: false,
      enableAutoBackup: false,
      isLoading: true,
    };
    this._isMounted = false;
    this.timerId = null;
    this.scheduleInformation = "";
  }

  componentDidMount() {
    this._isMounted = true;

    this.setBasicSettings();
  }

  setBasicSettings = async () => {
    const { t } = this.props;

    const requests = [enableRestore(), enableAutoBackup(), getBackupProgress()];

    try {
      const [restore, autoBackup, progress] = await Promise.allSettled(
        requests
      );

      const backupProgress = progress.value;

      const canRestore = restore.value;
      const canAutoBackup = autoBackup.value;

      if (backupProgress && !backupProgress.error) {
        this._isMounted &&
          this.setState({
            downloadingProgress: backupProgress.progress,
          });
        if (backupProgress.progress !== 100) {
          this.timerId = setInterval(() => this.getProgress(), 5000);
        }
      }

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

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

  onClickLink = (e) => {
    const { history } = this.props;
    e.preventDefault();
    history.push(e.target.pathname);
  };
  getProgress = () => {
    const { t } = this.props;
    const { downloadingProgress } = this.state;

    getBackupProgress()
      .then((response) => {
        if (response) {
          if (response.error.length > 0 && response.progress !== 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${t("BackupCreatedError")}`);
            console.log("error", response.error);
            this.timerId = null;
            this.setState({
              downloadingProgress: 100,
            });
            return;
          }
          if (this._isMounted) {
            downloadingProgress !== response.progress &&
              this.setState({
                downloadingProgress: response.progress,
              });
          }
          if (response.progress === 100) {
            clearInterval(this.timerId);

            this.timerId && toastr.success(`${t("BackupCreatedSuccess")}`);
            this.timerId = null;
          }
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(`${t("BackupCreatedError")}`);
        console.log("err", err);
        this.timerId = null;
        if (this._isMounted) {
          this.setState({
            downloadingProgress: 100,
          });
        }
      });
  };

  onClickFloatingButton = () => {
    const { history } = this.props;
    history.push(
      combineUrl(proxyURL, "/settings/datamanagement/backup/manual-backup")
    );
  };
  render() {
    const { t, helpUrlCreatingBackup } = this.props;
    const {
      downloadingProgress,
      isLoading,
      enableRestore,
      enableAutoBackup,
    } = this.state;

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

            <Text className="schedule-information">
              {this.scheduleInformation}
            </Text>
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

export default inject(({ auth }) => {
  const { language } = auth;
  const { helpUrlCreatingBackup } = auth.settingsStore;
  return {
    helpUrlCreatingBackup,
    language,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupMobileView)));
