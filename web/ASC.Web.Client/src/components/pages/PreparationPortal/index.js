import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import {
  StyledPreparationPortal,
  StyledBodyPreparationPortal,
} from "./styledPreparationPortal";
import Text from "@appserver/components/text";
import { getRestoreProgress } from "../../../../../../packages/asc-web-common/api/portal";

class PreparationPortal extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      percent: 0,
      errorMessage: "",
    };
    this.timerId = null;
  }
  componentDidMount() {
    getRestoreProgress()
      .then((response) => {
        if (response) {
          const { error, progress } = response;
          if (!error) {
            this.setState({
              percent: progress,
            });
            if (progress !== 100)
              this.timerId = setInterval(() => this.getProgress(), 1000);
          } else {
            this.setState({
              errorMessage: error,
            });
          }
        }
      })
      .catch((err) =>
        this.setState({
          errorMessage: err,
        })
      );
  }
  componentWillUnmount() {
    clearInterval(this.timerId);
  }
  getProgress = () => {
    getRestoreProgress().then((response) => {
      if (response) {
        const { error, progress } = response;
        if (!error) {
          this.setState({
            percent: progress,
          });

          if (progress === 100) {
            clearInterval(this.timerId);
          }
        } else {
          clearInterval(this.timerId);
          this.setState({
            errorMessage: error,
          });
        }
      } else {
        clearInterval(this.timerId);
      }
    });
  };
  render() {
    const { t } = this.props;
    const { percent, errorMessage } = this.state;

    return (
      <StyledPreparationPortal>
        <ErrorContainer
          headerText={t("PreparationPortalTitle")}
          bodyText={t("PreparationPortalDescription")}
        >
          <StyledBodyPreparationPortal
            percent={percent}
            errorMessage={errorMessage}
          >
            {errorMessage ? (
              <Text
                className="preparation-portal_error"
                color="#F21C0E"
              >{`${errorMessage}`}</Text>
            ) : (
              <>
                <div className="preparation-portal_progress-bar">
                  <div className="preparation-portal_progress-line"></div>
                </div>
                <Text className="preparation-portal_percent">{`${percent}%`}</Text>
              </>
            )}
          </StyledBodyPreparationPortal>
        </ErrorContainer>
      </StyledPreparationPortal>
    );
  }
}
const PreparationPortalWrapper = withTranslation("PreparationPortal")(
  PreparationPortal
);
export default () => (
  <I18nextProvider i18n={i18n}>
    <PreparationPortalWrapper />
  </I18nextProvider>
);
