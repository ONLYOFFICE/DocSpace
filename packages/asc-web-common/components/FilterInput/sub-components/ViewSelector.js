import React from "react";
import PropTypes from "prop-types";
import SwitchButton from "@appserver/components/switch-button";
import { StyledViewSelector } from "../StyledFilterInput";

class ViewSelector extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      viewAs: props.viewAs,
    };
  }

  onClickViewSelector = (item) => {
    this.props.onClickViewSelector &&
      this.props.onClickViewSelector(item.target.checked);

    //this.props.onClickViewSelector && this.props.onClickViewSelector(item);
  };

  render() {
    const { isDisabled, viewAs } = this.props;

    const isChecked = viewAs === "tile" || false;

    return (
      <StyledViewSelector isDisabled={isDisabled}>
        <SwitchButton
          disabled={isDisabled}
          onChange={this.onClickViewSelector}
          checked={isChecked}
        />
      </StyledViewSelector>
    );
  }
}
ViewSelector.propTypes = {
  isDisabled: PropTypes.bool,
  viewAs: PropTypes.string,
  onClickViewSelector: PropTypes.func,
};

ViewSelector.defaultProps = {
  isDisabled: false,
};

export default ViewSelector;
