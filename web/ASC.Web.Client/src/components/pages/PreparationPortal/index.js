import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import StyledPreparationPortal from "./styledPreparationPortal";
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
    this.setState({ isLoadingData: true }, function () {
      getRestoreProgress()
        .then((response) => {
          if (response) {
            if (!response.error) {
              if (response.progress === 100)
                this.setState({
                  percent: 100,
                });
              if (response.progress !== 100)
                this.timerId = setInterval(() => this.getProgress(), 1000);
            } else {
              this.setState({
                errorMessage: response.error,
              });
            }
          }
        })
        .catch((err) =>
          this.setState({
            errorMessage: err,
          })
        )
        .finally(() => this.setState({ isLoadingData: false }));
    });
  }
  componentWillUnmount() {
    clearInterval(this.timerId);
  }
  getProgress = () => {
    getRestoreProgress().then((response) => {
      if (response) {
        if (!response.error) {
          const percent = response.progress;

          this.setState({
            percent: percent,
          });

          if (percent === 100) {
            clearInterval(this.timerId);
          }
        } else {
          clearInterval(this.timerId);
          this.setState({
            errorMessage: response.error,
          });
        }
      }
    });
  };
  render() {
    const { t } = this.props;
    const { percent, errorMessage, isLoadingData } = this.state;
    console.log("percent", errorMessage);
    return (
      <ErrorContainer
        headerText={t("PreparationPortalTitle")}
        bodyText={t("PreparationPortalDescription")}
      >
        {!isLoadingData ? (
          <StyledPreparationPortal
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
                <Text
                  className="preparation-portal_percent"
                  color="#a3a9ae"
                >{`${percent}%`}</Text>
              </>
            )}
          </StyledPreparationPortal>
        ) : (
          <></>
        )}
      </ErrorContainer>
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
