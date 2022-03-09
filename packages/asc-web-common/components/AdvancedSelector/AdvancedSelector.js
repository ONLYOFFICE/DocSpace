import React from "react";
import PropTypes from "prop-types";

import Selector from "./sub-components/Selector";
import Backdrop from "@appserver/components/backdrop";
import Aside from "@appserver/components/aside";

const sizes = ["compact", "full"];

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
  }

  onClose = (e) => {
    //console.log("onClose");
    //this.setState({ isOpen: false });
    this.props.onCancel && this.props.onCancel(e);
  };

  render() {
    const {
      isOpen,
      id,
      className,
      style,
      withoutAside,
      isDefaultDisplayDropDown,
      smallSectionWidth,
    } = this.props;

    return (
      <div id={id} className={className} style={style}>
        {withoutAside ? (
          <Selector {...this.props} />
        ) : (
          <>
            <Backdrop
              onClick={this.onClose}
              visible={isOpen}
              zIndex={310}
              isAside={true}
            />
            <Aside visible={isOpen} scale={false} className="aside-container">
              <Selector {...this.props} />
            </Aside>
          </>
        )}
      </div>
    );
  }
}

AdvancedSelector.propTypes = {
  id: PropTypes.string,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
  options: PropTypes.array,
  selectedOptions: PropTypes.array,
  groups: PropTypes.array,
  selectedGroups: PropTypes.array,

  value: PropTypes.string,
  placeholder: PropTypes.string,
  selectAllLabel: PropTypes.string,
  buttonLabel: PropTypes.string,

  size: PropTypes.oneOf(sizes),

  maxHeight: PropTypes.number,

  isMultiSelect: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedAll: PropTypes.bool,
  isOpen: PropTypes.bool,
  allowGroupSelection: PropTypes.bool,
  allowCreation: PropTypes.bool,
  allowAnyClickClose: PropTypes.bool,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  withoutAside: PropTypes.bool,

  onSearchChanged: PropTypes.func,
  onSelect: PropTypes.func,
  onGroupChange: PropTypes.func,
  onCancel: PropTypes.func,
  onAddNewClick: PropTypes.func,
  loadNextPage: PropTypes.func,
  isDefaultDisplayDropDown: PropTypes.bool,
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "full",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowGroupSelection: false,
  allowAnyClickClose: true,
  options: [],
  isDefaultDisplayDropDown: true,
};

export default AdvancedSelector;
