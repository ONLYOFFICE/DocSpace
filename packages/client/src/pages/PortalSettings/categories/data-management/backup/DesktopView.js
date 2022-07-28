import React from "react";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import FloatingButton from "@docspace/common/components/FloatingButton";
import {
  enableAutoBackup,
  enableRestore,
  getBackupSchedule,
} from "@docspace/common/api/portal";
import Loader from "@docspace/components/loader";
import { StyledBackup } from "./StyledBackup";
import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import RestoreBackup from "./restore-backup";
import toastr from "@docspace/components/toast/toastr";
import moment from "moment";
import { getBackupStorage } from "@docspace/common/api/settings";
import HelpButton from "@docspace/components/help-button";
import { getThirdPartyCommonFolderTree } from "@docspace/common/api/files";
class BackupDesktopView extends React.Component {
  constructor(props) {
    super(props);
    const { language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this.state = {
      enableRestore: false,
      enableAutoBackup: false,
      isInitialLoading: true,
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
    const {
      setThirdPartyStorage,
      setBackupSchedule,
      setCommonThirdPartyList,
      getProgress,
      t,
    } = this.props;

    const requests = [
      enableRestore(),
      enableAutoBackup(),
      getBackupSchedule(),
      getBackupStorage(),
      getThirdPartyCommonFolderTree(),
    ];

    try {
      getProgress(t);

      const [
        canRestore,
        canAutoBackup,
        backupSchedule,
        backupStorage,
        commonThirdPartyList,
      ] = await Promise.all(requests);

      setThirdPartyStorage(backupStorage);
      setBackupSchedule(backupSchedule);
      commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);

      this.setState({
        isInitialLoading: false,
        enableRestore: canRestore,
        enableAutoBackup: canAutoBackup,
      });
    } catch (error) {
      toastr.error(error);
      this.setState({
        isInitialLoading: false,
      });
    }
  };

  onClickLink = (e) => {
    const { history } = this.props;
    e.preventDefault();
    history.push(e.target.pathname);
  };

  render() {
    const {
      t,
      history,
      helpUrlCreatingBackup,
      downloadingProgress,
      organizationName,
      buttonSize,
      theme,
    } = this.props;
    const { isInitialLoading, enableRestore, enableAutoBackup } = this.state;

    const renderTooltip = (helpInfo) => {
      return (
        <>
          <HelpButton
            iconName={"/static/images/help.react.svg"}
            tooltipContent={
              <>
                <Trans t={t} i18nKey={`${helpInfo}`} ns="Settings">
                  {helpInfo}
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
            }
          />
        </>
      );
    };

    return isInitialLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBackup isDesktop={true} theme={theme}>
        <div className="backup_modules-separation">
          <div className="backup_modules-header_wrapper">
            <Text className="backup_modules-header">{t("ManualBackup")}</Text>
            {renderTooltip(
              t("ManualBackupHelp") +
                " " +
                t("ManualBackupHelpNote", { organizationName })
            )}
          </div>
          <Text className="backup_modules-description">
            {t("ManualBackupDescription")}
          </Text>

          <ManualBackup buttonSize={buttonSize} />
        </div>

        {enableAutoBackup && (
          <div className="backup_modules-separation">
            <div className="backup_modules-header_wrapper">
              <Text className="backup_modules-header">{t("AutoBackup")}</Text>
              {renderTooltip(
                t("AutoBackupHelp") +
                  " " +
                  t("AutoBackupHelpNote", { organizationName })
              )}
            </div>
            <Text className="backup_modules-description">
              {t("AutoBackupDescription")}
            </Text>

            <AutoBackup buttonSize={buttonSize} />
          </div>
        )}

        {enableRestore && (
          <>
            <div className="backup_modules-header_wrapper">
              <Text className="backup_modules-header">
                {t("RestoreBackup")}
              </Text>
              {renderTooltip(
                t("RestoreBackupHelp") + " " + t("RestoreBackupHelpNote")
              )}
            </div>
            <RestoreBackup history={history} buttonSize={buttonSize} />
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
  const { language, settingsStore } = auth;
  const {
    helpUrlCreatingBackup,
    organizationName,
    isTabletView,
    theme,
  } = settingsStore;
  const {
    setThirdPartyStorage,
    setBackupSchedule,
    setCommonThirdPartyList,
    getProgress,
    clearProgressInterval,
    downloadingProgress,
  } = backup;

  const buttonSize = isTabletView ? "normal" : "small";
  return {
    theme,
    helpUrlCreatingBackup,
    language,
    setThirdPartyStorage,
    setBackupSchedule,
    setCommonThirdPartyList,
    getProgress,
    clearProgressInterval,
    downloadingProgress,
    organizationName,
    buttonSize,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupDesktopView)));
