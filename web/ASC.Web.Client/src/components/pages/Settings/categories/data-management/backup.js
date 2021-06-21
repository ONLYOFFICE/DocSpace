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
import { getBackupProgress } from "@appserver/common/api/portal";
import { StyledBackup } from "./styled-backup";

const { proxyURL } = AppServerConfig;

import toastr from "@appserver/components/toast/toastr";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

class Backup extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      downloadingProgress: false,
    };
    this._isMounted = false;
    this.timerId = null;
  }
  componentDidMount() {
    this._isMounted = true;

    getBackupProgress().then((response) => {
      if (response && !response.error) {
        this._isMounted &&
          this.setState({
            downloadingProgress: response.progress,
            link: response.link,
          });
        if (response.progress !== 100) {
          this.timerId = setInterval(() => this.getProgress(), 5000);
        }
      }
    });
  }

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

    getBackupProgress()
      .then((response) => {
        if (response) {
          if (response.error.length > 0 && response.progress !== 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${t("CopyingError")}`);
            //console.log("error", response.error);
            this.timerId = null;
            this.setState({
              downloadingProgress: 100,
            });
            return;
          }
          if (this._isMounted) {
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
        //console.log("err", err);

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
    const { downloadingProgress } = this.state;
    return (
      <StyledBackup>
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
          <Text className="category-item-description">
            {t("AutomaticBackupSettingsDescription")}
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
  const { helpUrlCreatingBackup } = auth.settingsStore;
  return {
    helpUrlCreatingBackup,
  };
})(observer(withTranslation(["Settings", "Common"])(Backup)));
