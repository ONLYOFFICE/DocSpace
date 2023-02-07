import { Component } from "react";
import PropTypes from "prop-types";
import ReactDOM from "react-dom";

class Portal extends Component {
  constructor(props) {
    super(props);

    this.state = {
      mounted: props.visible,
    };
  }

  domExist() {
    return !!(
      typeof window !== undefined &&
      window.document &&
      window.document.createElement
    );
  }

  componentDidMount() {
    if (this.domExist() && !this.state.mounted) {
      this.setState({ mounted: true });
    }
  }

  render() {
    return this.props.element && this.state.mounted
      ? ReactDOM.createPortal(
          this.props.element,
          this.props.appendTo || document.body
        )
      : null;
  }
}

Portal.propTypes = {
  element: PropTypes.any,
  appendTo: PropTypes.any,
  visible: PropTypes.bool,
};

Portal.defaultProps = {
  element: null,
  appendTo: null,
  visible: false,
};

export default Portal;
