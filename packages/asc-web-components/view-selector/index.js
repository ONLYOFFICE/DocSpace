import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import { StyledViewSelector, IconWrapper } from "./styled-view-selector";

const ViewSelector = ({
  isDisabled,
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

  const lastIndx = viewSettings && viewSettings.length - 1;

  return (
    <StyledViewSelector
      {...rest}
      isDisabled={isDisabled}
      onClick={onChangeViewHandler}
      countItems={viewSettings.length}
    >
      {viewSettings &&
        viewSettings.map((el, indx) => {
          const { value, icon } = el;

          return (
            <IconWrapper
              isDisabled={isDisabled}
              isChecked={viewAs === value}
              firstItem={indx === 0}
              lastItem={indx === lastIndx}
              key={value}
              className="view-selector-icon"
              data-view={value}
            >
              <ReactSVG src={icon} />
            </IconWrapper>
          );
        })}
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
