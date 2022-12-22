import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { withTranslation } from "react-i18next";

import {
  StyledPreparationPortal,
  StyledBodyPreparationPortal,
} from "./StyledPreparationPortal";
import Text from "@docspace/components/text";
import { getRestoreProgress } from "@docspace/common/api/portal";
import { observer, inject } from "mobx-react";
import PropTypes from "prop-types";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

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
      .catch((err) => {
        let errorMessage = "";
        if (typeof err === "object") {
          errorMessage =
            err?.response?.data?.error?.message ||
            err?.statusText ||
            err?.message ||
            "";
        } else {
          errorMessage = err;
        }

        this.setState({
          errorMessage: errorMessage,
        });
      });
  }
  componentWillUnmount() {
    clearInterval(this.timerId);
    clearInterval(this.progressTimerId);
  }

  progressInitiationFirstBound = () => {
    const { multiplicationFactor } = this.props;
    const { percent, firstBound } = this.state;

    let progress = percent;

    const common = baseFirstMultiplicationFactor * multiplicationFactor;

    if (!this.progressTimerId)
      this.progressTimerId = setInterval(() => {
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

    if (!this.progressTimerId)
      this.progressTimerId = setInterval(() => {
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
    const { percent } = this.state;
    let progress = percent;
    const common = baseThirdMultiplicationFactor * multiplicationFactor;
    if (!this.progressTimerId)
      this.progressTimerId = setInterval(() => {
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
    const { t, withoutHeader, style } = this.props;
    const { percent, errorMessage } = this.state;

    return (
      <StyledPreparationPortal>
        <ErrorContainer
          headerText={withoutHeader ? "" : t("Common:PreparationPortalTitle")}
          style={style}
        >
          <ColorTheme
            themeId={ThemeType.Progress}
            percent={percent}
            errorMessage={errorMessage}
            className="preparation-portal_body-wrapper"
          >
            {errorMessage ? (
              <Text
                className="preparation-portal_error"
                color="#F21C0E"
              >{`${errorMessage}`}</Text>
            ) : (
              <>
                <div className="preparation-portal_progress">
                  <div className="preparation-portal_progress-bar">
                    <div className="preparation-portal_progress-line"></div>
                  </div>
                  <Text className="preparation-portal_percent">{`${percent} %`}</Text>
                </div>
                <Text className="preparation-portal_text">
                  {t("PreparationPortalDescription")}
                </Text>
              </>
            )}
          </ColorTheme>
        </ErrorContainer>
      </StyledPreparationPortal>
    );
  }
}

const PreparationPortalWrapper = inject(({ backup }) => {
  const { backupSize } = backup;

  const multiplicationFactor = backupSize
    ? backupSize / baseSize
    : unSizeMultiplicationFactor;

  return {
    multiplicationFactor,
  };
})(
  withTranslation(["PreparationPortal", "Common"])(observer(PreparationPortal))
);

PreparationPortal.propTypes = {
  withoutHeader: PropTypes.bool,
};

PreparationPortal.defaultProps = {
  withoutHeader: false,
};

export default (props) => <PreparationPortalWrapper {...props} />;
