import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import { withTranslation } from "react-i18next";
import ArrowRightIcon from "../../../../../../public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import styled from "styled-components";
import FloatingButton from "@appserver/common/components/FloatingButton";
import { getBackupProgress } from "@appserver/common/api/portal";
import { Redirect } from "react-router-dom";

const { proxyURL } = AppServerConfig;

import toastr from "@appserver/components/toast/toastr";

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

    getBackupProgress().then((res) => {
      if (res) {
        this.setState({
          downloadingProgress: res.progress,
          link: res.link,
        });
        if (res.progress !== 100) {
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
      .then((res) => {
        if (res) {
          if (res.error.length > 0 && res.progress !== 100) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${res.error}`);
            console.log("error", res.error);
            this.timerId = null;
            return;
          }
          if (this._isMounted) {
            this.setState({
              downloadingProgress: res.progress,
            });
          }
          if (res.progress === 100) {
            clearInterval(this.timerId);

            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;
          }
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(err);
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
    const { t } = this.props;
    const { downloadingProgress } = this.state;
    return (
      <StyledComponent>
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
            <Text className="inherit-title-link">{t("DataRestore")}</Text>
            <StyledArrowRightIcon size="small" color="#333333" />
          </div>
          <Text className="category-item-description">
            {t("DataRestoreSettingsDescription")}
          </Text>
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
      </StyledComponent>
    );
  }
}
export default withTranslation("Settings")(Backup);
