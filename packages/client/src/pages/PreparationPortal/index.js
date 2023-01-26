import React, { useEffect, useState } from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { withTranslation } from "react-i18next";

import { StyledPreparationPortal } from "./StyledPreparationPortal";
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
const firstBound = 10,
  secondBound = 63;

let timerId = null,
  progressTimerId = null;

const PreparationPortal = (props) => {
  const { multiplicationFactor, t, withoutHeader, style } = props;

  const [percent, setPercent] = useState(0);
  const [errorMessage, setErrorMessage] = useState("");

  const clearAllIntervals = () => {
    clearInterval(timerId);
    clearInterval(progressTimerId);

    progressTimerId = null;
    timerId = null;
  };

  const returnToPortal = () => {
    setTimeout(() => {
      window.location.replace("/");
    }, 5000);
  };

  const progressInitiationFirstBound = () => {
    let progress = percent;

    const delay = baseFirstMultiplicationFactor * multiplicationFactor;

    if (progressTimerId) return;
    progressTimerId = setInterval(() => {
      progress += 1;

      if (progress !== firstBound) percent < progress && setPercent(progress);
      else {
        clearInterval(progressTimerId);
        progressTimerId = null;
      }
    }, delay);
  };

  const progressInitiationSecondBound = () => {
    let progress = percent;

    const delay = baseSecondMultiplicationFactor * multiplicationFactor;

    if (progressTimerId) return;
    progressTimerId = setInterval(() => {
      progress += 1;
      if (progress !== secondBound) percent < progress && setPercent(progress);
      else {
        clearInterval(progressTimerId);
        progressTimerId = null;
      }
    }, delay);
  };

  const progressInitiationThirdBound = () => {
    let progress = percent;
    const delay = baseThirdMultiplicationFactor * multiplicationFactor;

    if (progressTimerId) return;

    progressTimerId = setInterval(() => {
      progress += 1;

      if (progress < 98) percent < progress && setPercent(progress);
      else {
        clearInterval(progressTimerId);
        progressTimerId = null;
      }
    }, delay);
  };

  const getIntervalProgress = async () => {
    try {
      const response = await getRestoreProgress();

      if (response.error) {
        clearInterval(timerId);
        clearInterval(progressTimerId);

        progressTimerId = null;
        timerId = null;
        setErrorMessage(response.error);

        return;
      }

      const percentProgress = response.progress;

      if (percentProgress !== percent && percent < percentProgress) {
        setPercent(percentProgress);

        clearInterval(progressTimerId);
        progressTimerId = null;

        if (percentProgress < secondBound) {
          progressInitiationSecondBound();
        } else {
          progressInitiationThirdBound();
        }
      }

      if (percentProgress === 100) {
        clearAllIntervals();
        returnToPortal();
      }
    } catch (error) {
      clearAllIntervals();
      setErrorMessage(error);
    }
  };

  const getRecoveryProgress = async () => {
    const errorMessage = (error) => {
      if (typeof error !== "object") return error;

      return (
        err?.response?.data?.error?.message ||
        err?.statusText ||
        err?.message ||
        ""
      );
    };

    try {
      const response = await getRestoreProgress();

      if (!response) {
        setErrorMessage(t("Common:ErrorInternalServer"));
        return;
      }

      if (response.error) {
        setErrorMessage(response.error);

        return;
      }

      if (response.progress === 100) {
        setPercent(100);
        returnToPortal();
      } else {
        timerId = setInterval(() => getIntervalProgress(), 1000);
        progressInitiationFirstBound();
      }
    } catch (err) {
      setErrorMessage(errorMessage(err));
    }
  };
  useEffect(async () => {
    setTimeout(() => {
      getRecoveryProgress();
    }, 4000);

    return () => {
      clearAllIntervals();
    };
  }, []);

  const headerText = errorMessage
    ? t("Common:Error")
    : t("Common:PreparationPortalTitle");

  return (
    <StyledPreparationPortal>
      <ErrorContainer
        headerText={withoutHeader ? "" : headerText}
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
};

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
