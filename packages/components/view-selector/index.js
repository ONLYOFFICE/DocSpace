import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import { StyledViewSelector, IconWrapper } from "./styled-view-selector";
import { useTranslation } from "react-i18next";

const ViewSelector = ({
  isDisabled,
  isFilter,
  viewSettings,
  viewAs,
  onChangeView,
  ...rest
}) => {
  const onChangeViewHandler = (e) => {
    if (isDisabled) return;
    const el = e.target.closest(".view-selector-icon");

    const view = el.dataset.view;
    if (view !== viewAs) {
      const option = viewSettings.find((setting) => view === setting.value);
      option.callback && option.callback();
      onChangeView(view);
    }
  };

  const renderFewIconView = () => {
    return viewSettings.map((el, indx) => {
      const { value, icon } = el;

      return (
        <IconWrapper
          isDisabled={isDisabled}
          isChecked={viewAs === value}
          firstItem={indx === 0}
          lastItem={indx === lastIndx}
          key={value}
          name={`view-selector-name_${value}`}
          className="view-selector-icon"
          data-view={value}
          title={
            value === "row"
              ? t("Common:SwitchViewToCompact")
              : t("Common:SwitchToThumbnails")
          }
        >
          <ReactSVG src={icon} />
        </IconWrapper>
      );
    });
  };

  const renderOneIconView = () => {
    const element = viewSettings.find((el) => el.value != viewAs);

    if (element) {
      const { value, icon } = element;

      return (
        <IconWrapper
          isFilter={isFilter}
          isDisabled={isDisabled}
          key={value}
          name={`view-selector-name_${value}`}
          className="view-selector-icon"
          data-view={value}
          title={
            value === "row"
              ? t("Common:SwitchViewToCompact")
              : t("Common:SwitchToThumbnails")
          }
        >
          <ReactSVG src={icon} />
        </IconWrapper>
      );
    }

    return <></>;
  };

  const lastIndx = viewSettings && viewSettings.length - 1;

  const { t } = useTranslation("Common");

  return (
    <StyledViewSelector
      {...rest}
      isDisabled={isDisabled}
      onClick={onChangeViewHandler}
      countItems={viewSettings.length}
      isFilter={isFilter}
    >
      {viewSettings ? (
        isFilter ? (
          renderOneIconView()
        ) : (
          renderFewIconView()
        )
      ) : (
        <></>
      )}
    </StyledViewSelector>
  );
};

ViewSelector.propTypes = {
  /* Disables the button default functionality */
  isDisabled: PropTypes.bool,
  /* The event triggered when the button is clicked  */
  onChangeView: PropTypes.func,
  /* Object containing view settings  */
  viewSettings: PropTypes.arrayOf(PropTypes.object).isRequired,
  /* Current application view */
  viewAs: PropTypes.string.isRequired,
};

export default ViewSelector;
