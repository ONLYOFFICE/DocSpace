import React, { useState } from "react";
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
  const [currentView, setCurrentView] = useState(viewAs);

  const onChangeViewHandler = (e) => {
    if (isDisabled) return;
    const el = e.target.closest(".view-selector-icon");

    const view = el.dataset.view;
    if (view !== currentView) {
      const option = viewSettings.find((setting) => view === setting.key);
      option.callback && option.callback();
      setCurrentView(view);
      onChangeView(view);
    }
  };

  const lastIndx = viewSettings && viewSettings.length - 1;

  return (
    <StyledViewSelector
      {...rest}
      isDisabled={isDisabled}
      onClick={onChangeViewHandler}
    >
      {viewSettings &&
        viewSettings.map((el, indx) => {
          const { key, icon } = el;

          return (
            <IconWrapper
              isDisabled={isDisabled}
              isChecked={currentView === key}
              firstItem={indx === 0}
              lastItem={indx === lastIndx}
              key={key}
              className="view-selector-icon"
              data-view={key}
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
