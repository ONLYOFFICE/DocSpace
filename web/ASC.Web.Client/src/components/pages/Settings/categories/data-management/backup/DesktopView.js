import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import FloatingButton from "@appserver/common/components/FloatingButton";
import {
  enableAutoBackup,
  enableRestore,
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
      SelectFolderDialog.getCommonThirdPartyList(),
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
    } = this.props;
    const { isInitialLoading, enableRestore, enableAutoBackup } = this.state;

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

    return isInitialLoading ? (
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

          <ManualBackup />
        </div>

        {enableAutoBackup && (
          <div className="backup_modules-separation">
            <div className="backup_modules-header_wrapper">
              <Text className="backup_modules-header">{t("AutoBackup")}</Text>
              {renderTooltip(t("AutoBackupHelp"))}
            </div>
            <Text className="backup_modules-description">
              {t("AutoBackupDescription")}
            </Text>

            <AutoBackup />
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
            <RestoreBackup history={history} />
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
  const {
    setThirdPartyStorage,
    setBackupSchedule,
    setCommonThirdPartyList,
    getProgress,
    clearProgressInterval,
    downloadingProgress,
  } = backup;

  return {
    helpUrlCreatingBackup,
    language,
    setThirdPartyStorage,
    setBackupSchedule,
    setCommonThirdPartyList,
    getProgress,
    clearProgressInterval,
    downloadingProgress,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupDesktopView)));
