import React from "react";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import ArrowRightIcon from "@docspace/client/public/images/arrow.right.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import Box from "@docspace/components/box";
import styled from "styled-components";
import FloatingButton from "@docspace/common/components/FloatingButton";
import { enableAutoBackup, enableRestore } from "@docspace/common/api/portal";
import { getBackupStorage } from "@docspace/common/api/settings";
import Loader from "@docspace/components/loader";
import { StyledBackup, StyledSettingsHeader } from "./StyledBackup";
import Headline from "@docspace/common/components/Headline";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";
import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import RestoreBackup from "./restore-backup";
import { LayoutContextConsumer } from "SRC_DIR/components/Layout/context";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

class BackupMobileView extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      enableRestore: false,
      enableAutoBackup: false,
      isLoading: true,
      autoBackup: false,
      manualBackup: false,
      restoreBackup: false,
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
    const { t, getProgress, setThirdPartyStorage } = this.props;

    try {
      const requests = [
        enableRestore(),
        enableAutoBackup(),
        getBackupStorage(),
      ];

      getProgress(t);

      const [canRestore, canAutoBackup, backupStorage] = await Promise.all(
        requests
      );

      setThirdPartyStorage(backupStorage);

      this.setState({
        isLoading: false,
        enableRestore: canRestore,
        enableAutoBackup: canAutoBackup,
      });
    } catch (error) {
      toastr.error(error);
      this.setState({
        isLoading: false,
      });
    }
  };

  onClickFloatingButton = () => {
    const { manualBackup } = this.state;

    !manualBackup &&
      this.setState({
        manualBackup: true,
        autoBackup: false,
        restoreBackup: false,
      });
  };

  onClickModule = (name) => {
    this.setState({
      [name]: true,
    });
  };

  onBackToParent = () => {
    this.setState({
      autoBackup: false,
      manualBackup: false,
      restoreBackup: false,
    });
  };
  render() {
    const {
      t,
      helpUrlCreatingBackup,
      downloadingProgress,
      history,
    } = this.props;
    const {
      isLoading,
      enableRestore,
      enableAutoBackup,
      autoBackup,
      manualBackup,
      restoreBackup,
    } = this.state;

    const autoBackupTitle = t("AutoBackup");
    const manualBackupTitle = t("ManualBackup");
    const restoreBackupTitle = t("RestoreBackup");

    const isSectionOpen = autoBackup || manualBackup || restoreBackup;
    const headerTitle = autoBackup
      ? autoBackupTitle
      : manualBackup
      ? manualBackupTitle
      : restoreBackup
      ? restoreBackupTitle
      : "";
    const buttonSize = "normal";
    const renderSection = (section, sectionTitle, keyHelp) => (
      <div className="backup-section_wrapper">
        <div className="backup-section_heading">
          <Text
            className="backup-section_text hidingHeader"
            onClick={() => this.onClickModule(section)}
          >
            {sectionTitle}
            <StyledArrowRightIcon
              className="backup-section_arrow-button"
              size="small"
              color="#333333"
            />
          </Text>
        </div>

        <Trans
          t={t}
          i18nKey={keyHelp}
          ns="Settings"
          components={{ strong: <></> }}
        />

        <Box marginProp="10px 0 0 0">
          <Link
            color="#316DAA"
            target="_blank"
            isHovered={true}
            href={helpUrlCreatingBackup}
            fontWeight={600}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
      </div>
    );
    const renderBackupSections = () => (
      <>
        {renderSection("manualBackup", manualBackupTitle, "ManualBackupHelp")}

        {enableAutoBackup &&
          renderSection("autoBackup", autoBackupTitle, "AutoBackupHelp")}

        {enableRestore &&
          renderSection(
            "restoreBackup",
            restoreBackupTitle,
            "RestoreBackupHelp"
          )}
      </>
    );

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <LayoutContextConsumer>
        {(value) => (
          <StyledBackup>
            {isSectionOpen && (
              <StyledSettingsHeader isVisible={value.isVisible}>
                <IconButton
                  iconName="/static/images/arrow.path.react.svg"
                  size="17"
                  color="#A3A9AE"
                  hoverColor="#657077"
                  isFill={true}
                  onClick={this.onBackToParent}
                  className="backup_arrow-button"
                />

                <Headline
                  type="content"
                  className="backup_header"
                  truncate={true}
                >
                  {headerTitle}
                </Headline>
              </StyledSettingsHeader>
            )}

            {autoBackup && <AutoBackup buttonSize={buttonSize} />}
            {manualBackup && <ManualBackup buttonSize={buttonSize} />}
            {restoreBackup && (
              <RestoreBackup history={history} buttonSize={buttonSize} />
            )}

            {!isSectionOpen && renderBackupSections()}

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
        )}
      </LayoutContextConsumer>
    );
  }
}

export default inject(({ auth, backup }) => {
  const { helpUrlCreatingBackup } = auth.settingsStore;
  const {
    getProgress,
    downloadingProgress,
    clearProgressInterval,
    setThirdPartyStorage,
  } = backup;
  return {
    helpUrlCreatingBackup,

    getProgress,
    downloadingProgress,
    clearProgressInterval,
    setThirdPartyStorage,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupMobileView)));
