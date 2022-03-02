import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import {
  StyledPreparationPortal,
  StyledBodyPreparationPortal,
} from "./styledPreparationPortal";
import Text from "@appserver/components/text";
import {
  getBackupProgress,
  getRestoreProgress,
} from "../../../../../../packages/asc-web-common/api/portal";
import { observer, inject } from "mobx-react";

const baseSize = 1073741824; //number of bytes in one GB
const unSizeMultiplicationFactor = 3;
const baseFirstMultiplicationFactor = 700;
const baseSecondMultiplicationFactor = 400;
const baseThirdMultiplicationFactor = 180;

class PreparationPortal extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      percent: 0,
      errorMessage: "",
      firstBound: 10,
      secondBound: 63,
    };
    this.timerId = null;
    this.progressTimerId = null;
  }
  componentDidMount() {
    this.props.getSettings();

    // this.start = new Date().getTime();
    // console.log("this.start", this.start);

    getRestoreProgress()
      .then((response) => {
        if (response) {
          if (!response.error) {
            if (response.progress === 100)
              this.setState({
                percent: 100,
              });
            if (response.progress !== 100) {
              this.timerId = setInterval(() => this.getProgress(), 1000);
              this.progressInitiationFirstBound();
            }
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
      );
  }
  componentWillUnmount() {
    clearInterval(this.timerId);
  }

  progressInitiationFirstBound = () => {
    const { multiplicationFactor } = this.props;
    const { percent, firstBound } = this.state;

    let progress = percent;

    const common = baseFirstMultiplicationFactor * multiplicationFactor;

    console.log("common", common, "percent", percent);
    if (!this.progressTimerId)
      this.progressTimerId = setInterval(() => {
        console.log("progressInitiationFirstBound");
        progress += 1;
        if (progress !== firstBound && percent < progress)
          percent < progress &&
            this.setState({
              percent: progress,
            });
        else {
          clearInterval(this.progressTimerId);
          this.progressTimerId = null;
        }
      }, common);
  };
  progressInitiationSecondBound = () => {
    const { multiplicationFactor } = this.props;
    const { percent, secondBound } = this.state;

    let progress = percent;

    const common = baseSecondMultiplicationFactor * multiplicationFactor;

    console.log("common", common, "percent", percent);
    if (!this.progressTimerId)
      this.progressTimerId = setInterval(() => {
        console.log("progressInitiationSecondBound");
        progress += 1;
        if (progress !== secondBound)
          percent < progress &&
            this.setState({
              percent: progress,
            });
        else {
          clearInterval(this.progressTimerId);
          this.progressTimerId = null;
        }
      }, common);
  };

  progressInitiationThirdBound = () => {
    const { multiplicationFactor } = this.props;
    const { percent, secondBound } = this.state;
    let progress = percent;
    const common = baseThirdMultiplicationFactor * multiplicationFactor;
    if (!this.progressTimerId)
      this.progressTimerId = setInterval(() => {
        //console.log("progressInitiationThirdBound", percent);
        progress += 1;

        if (progress < 98)
          percent < progress &&
            this.setState({
              percent: progress,
            });
        else {
          clearInterval(this.progressTimerId);
          this.progressTimerId = null;
        }
      }, common);
  };
  getProgress = () => {
    const { secondBound } = this.state;
    getRestoreProgress()
      .then((response) => {
        if (response) {
          if (!response.error) {
            const percentProgress = response.progress;

            percentProgress !== this.state.percent &&
              this.state.percent < percentProgress &&
              this.setState(
                {
                  percent: percentProgress,
                },
                () => {
                  clearInterval(this.progressTimerId);
                  this.progressTimerId = null;

                  if (percentProgress < secondBound) {
                    this.progressInitiationSecondBound();
                  } else {
                    console.log("percentProgress", percentProgress);
                    this.progressInitiationThirdBound();
                  }
                }
              );

            if (percentProgress === 100) {
              clearInterval(this.timerId);
              clearInterval(this.progressTimerId);

              this.progressTimerId = null;
              this.timerId = null;
            }
          } else {
            clearInterval(this.timerId);
            clearInterval(this.progressTimerId);

            this.progressTimerId = null;
            this.timerId = null;

            this.setState({
              errorMessage: response.error,
            });
          }
        }
      })
      .catch((e) => {
        clearInterval(this.timerId);
        clearInterval(this.progressTimerId);

        this.progressTimerId = null;
        this.timerId = null;

        this.setState({
          percent: 100,
        });
      });
  };
  render() {
    const { t } = this.props;
    const { percent, errorMessage, isLoadingData } = this.state;
    console.log("percent", errorMessage);
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
                <Text
                  className="preparation-portal_percent"
                  color="#a3a9ae"
                >{`${percent}%`}</Text>
              </>
            )}
          </StyledBodyPreparationPortal>
        </ErrorContainer>
      </StyledPreparationPortal>
    );
  }
}
// const PreparationPortalWrapper = withTranslation("PreparationPortal")(
//   PreparationPortal
// );

const PreparationPortalWrapper = inject(({ auth, backup }) => {
  const { backupSize } = backup;

  const multiplicationFactor = backupSize
    ? backupSize / baseSize
    : unSizeMultiplicationFactor;

  return {
    getSettings: auth.settingsStore.getSettings,
    multiplicationFactor,
  };
})(withTranslation("PreparationPortal")(observer(PreparationPortal)));
export default () => (
  <I18nextProvider i18n={i18n}>
    <PreparationPortalWrapper />
  </I18nextProvider>
);
