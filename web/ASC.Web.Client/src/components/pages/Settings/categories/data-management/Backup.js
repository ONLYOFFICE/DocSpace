import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import ArrowRightIcon from "../../../../../../public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import Box from "@appserver/components/box";
import styled from "styled-components";
import FloatingButton from "@appserver/common/components/FloatingButton";
import {
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

class Backup extends React.Component {
  constructor(props) {
    super(props);
    const { language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this.state = {
      downloadingProgress: 100,
      enableRestore: false,
      isLoading: false,
    };
    this._isMounted = false;
    this.timerId = null;
    this.scheduleInformation = "";
  }

  componentDidMount() {
    this._isMounted = true;

    this.setState(
      {
        isLoading: true,
      },
      function () {
        this.setBasicSettings();
      }
    );
  }

  setBasicSettings = async () => {
    const { t } = this.props;

    const requests = [
      enableRestore(),
      getBackupProgress(),
      getBackupSchedule(),
    ];

    let progress, schedule, enable;

    [enable, progress, schedule] = await Promise.allSettled(requests);

    const backupProgress = progress.value;
    const backupSchedule = schedule.value;

    if (backupProgress) {
      if (!backupProgress.error) {
        this._isMounted &&
          this.setState({
            downloadingProgress: backupProgress.progress,
            link: backupProgress.link,
          });
        if (backupProgress.progress !== 100) {
          this.timerId = setInterval(() => this.getProgress(), 5000);
        }
      }
    }

    if (backupSchedule) {
      if (backupSchedule.storageType === 0)
        this.scheduleInformation += `${t("DocumentsModule")} `;
      if (backupSchedule.storageType === 1)
        this.scheduleInformation += `${t("ThirdPartyResource")} `;
      if (backupSchedule.storageType === 5)
        this.scheduleInformation += `${t("ThirdPartyStorage")} `;
      let time = backupSchedule.cronParams.hour;
      let day = backupSchedule.cronParams.day;

      if (backupSchedule.cronParams.period === 1) {
        let isoWeekDay = day !== 1 ? day - 1 : 7;

        this.scheduleInformation += `(${t(
          "WeeklyPeriodSchedule"
        )}, ${moment()
          .isoWeekday(isoWeekDay)
          .add(7, "days")
          .hour(time)
          .minute("00")
          .format("dddd,  LT")})`;
      }

      if (backupSchedule.cronParams.period === 0) {
        this.scheduleInformation += `(${t(
          "DailyPeriodSchedule"
        )}, ${moment().add(1, "days").hour(time).minute("00").format("LT")})`;
      }

      if (backupSchedule.cronParams.period === 2) {
        const year = moment().year();
        const month = moment().month();

        this.scheduleInformation += `(${t("MonthlyPeriodSchedule")}, ${moment([
          year,
          0,
          day,
        ])
          .month(month)
          .hour(time)
          .minute("00")
          .format("Do, LT")})`;
      }
    }
    this.setState({
      isLoading: false,
      enableRestore: enable,
    });
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
            this.timerId && toastr.error(`${t("CopyingError")}`);
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

            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;
          }
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(`${t("CopyingError")}`);
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
    const { downloadingProgress, isLoading, enableRestore } = this.state;

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBackup>
        {enableRestore && (
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
                {t("AutomaticBackup")}
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
              {t("DataRestore")}
            </Link>
            <StyledArrowRightIcon size="small" color="#333333" />
          </div>
          <Text className="category-item-description">
            {t("DataRestoreSettingsDescription")}
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
})(observer(withTranslation(["Settings", "Common"])(Backup)));
