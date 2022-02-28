import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import { withTranslation, Trans } from "react-i18next";
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
import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import RestoreBackup from "./restore-backup";
import SelectFolderDialog from "files/SelectFolderDialog";
import Tooltip from "@appserver/components/tooltip";
import toastr from "@appserver/components/toast/toastr";
import moment from "moment";
import { getBackupStorage } from "@appserver/common/api/settings";
import HelpIcon from "../../../../../../../../../packages/asc-web-components/public/static/images/help.react.svg";

class BackupDesktopView extends React.Component {
  constructor(props) {
    super(props);
    const { language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this.state = {
      downloadingProgress: 100,
      enableRestore: false,
      enableAutoBackup: false,
      backupSchedule: {},
      commonThirdPartyList: {},
      isLoading: true,
    };
    this._isMounted = false;
    this.timerId = null;
  }

  componentDidMount() {
    this._isMounted = true;

    this.setBasicSettings();
  }

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }
  setBasicSettings = async () => {
    const { setThirdPartyStorage } = this.props;

    const requests = [
      enableRestore(),
      enableAutoBackup(),
      getBackupProgress(),
      getBackupSchedule(),
      getBackupStorage(),
      SelectFolderDialog.getCommonThirdPartyList(),
    ];

    try {
      const [
        canRestore,
        canAutoBackup,
        backupProgress,
        backupSchedule,
        backupStorage,
        commonThirdPartyList,
      ] = await Promise.all(requests);

      if (backupProgress && !backupProgress.error) {
        this._isMounted &&
          this.setState({
            downloadingProgress: backupProgress.progress,
          });
        if (backupProgress.progress !== 100) {
          this.timerId = setInterval(() => this.getProgress(), 5000);
        }
      }

      setThirdPartyStorage(backupStorage);

      this.setState({
        isLoading: false,
        enableRestore: canRestore,
        enableAutoBackup: canAutoBackup,
        backupSchedule,
        commonThirdPartyList,
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

  setBackupProgress = () => {
    this.setState({
      downloadingProgress: 1,
    });

    if (!this.timerId) {
      this.timerId = setInterval(() => this.getProgress(), 1000);
    }
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

            this._isMounted &&
              this.setState({
                ...(response.link &&
                  response.link.slice(0, 1) === "/" && { link: response.link }),
              });

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

  render() {
    const { t, history, helpUrlCreatingBackup } = this.props;
    const {
      downloadingProgress,
      isLoading,
      enableRestore,
      enableAutoBackup,
      backupSchedule,
      commonThirdPartyList,
      link,
    } = this.state;

    const renderTooltip = (helpInfo) => {
      return (
        <>
          <HelpIcon size="medium" data-tip={helpInfo} data-for="help-tooltip" />
          <Tooltip
            id="help-tooltip"
            offsetTop={0}
            getContent={(dataTip) => (
              <>
                <Trans t={t} i18nKey={`${helpInfo}`} ns="Settings">
                  {dataTip}
                </Trans>
                <div>
                  <Link
                    as="a"
                    href={helpUrlCreatingBackup}
                    target="_blank"
                    color="#555F65"
                    isBold
                    isHovered
                  >
                    {t("Common:LearnMore")}
                  </Link>
                </div>
              </>
            )}
            effect="float"
            place="right"
            maxWidth="320px"
            color="#F8F7BF"
          />
        </>
      );
    };

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBackup isDesktop={true}>
        <div className="backup_modules-separation">
          <div className="backup_modules-header_wrapper">
            <Text className="backup_modules-header">{t("ManualBackup")}</Text>
            {renderTooltip(t("ManualBackupHelp"))}
          </div>
          <Text className="backup_modules-description">
            {t("ManualBackupDescription")}
          </Text>

          <ManualBackup
            isDesktop
            setBackupProgress={this.setBackupProgress}
            isCopyingLocal={downloadingProgress}
            temporaryLink={link}
          />
        </div>

        {enableAutoBackup && (
          <div className="backup_modules-separation">
            <div className="backup_modules-header_wrapper">
              <Text className="backup_modules-header">
                {t("AutomaticBackup")}
              </Text>
              {renderTooltip(t("AutoBackupHelp"))}
            </div>
            <Text className="backup_modules-description">
              {t("AutoBackupDescription")}
            </Text>

            <AutoBackup
              backupSchedule={backupSchedule}
              commonThirdPartyList={commonThirdPartyList}
            />
          </div>
        )}

        {enableRestore && (
          <>
            <div className="backup_modules-header_wrapper">
              <Text className="backup_modules-header">
                {t("RestoreBackup")}
              </Text>
              {renderTooltip(t("RestoreBackupHelp"))}
            </div>
            <RestoreBackup
              isDesktop
              history={history}
              isCopyingLocal={downloadingProgress}
            />
          </>
        )}

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
          />
        )}
      </StyledBackup>
    );
  }
}

export default inject(({ auth, backup }) => {
  const { language } = auth;
  const { helpUrlCreatingBackup } = auth.settingsStore;
  const { setThirdPartyStorage } = backup;

  return {
    helpUrlCreatingBackup,
    language,
    setThirdPartyStorage,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupDesktopView)));
