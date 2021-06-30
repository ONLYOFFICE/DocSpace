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
      isError: false,
    };
    this.timerId = null;
  }
  componentDidMount() {
    this.timerId = setInterval(() => this.getProgress, 1000);
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
            isError: true,
          });
        }
      }
    });
  };
  render() {
    const { t } = this.props;
    const { percent } = this.state;
    return (
      <ErrorContainer
        headerText={t("PreparationPortalTitle")}
        bodyText={t("PreparationPortalDescription")}
      >
        <StyledPreparationPortal percent={percent}>
          <div className="preparation-portal_progress-bar">
            <div className="preparation-portal_progress-line"></div>
          </div>
          <Text
            className="preparation-portal_percent"
            color="#a3a9ae"
          >{`${percent}%`}</Text>
        </StyledPreparationPortal>
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
