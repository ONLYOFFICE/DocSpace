import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Backdrop from "../backdrop";
import Aside from "../aside";
import Heading from "../heading";
import { desktop } from "../../utils/device";
import throttle from "lodash/throttle";
import { Icons } from "../icons";

const Dialog = styled.div`
  position: relative;
  width: auto;
  max-width: 560px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  min-height: 100%;
`;

const Content = styled.div`
  position: relative;
  width: 100%;
  background-color: #fff;
  padding: 0 16px 16px;
  box-sizing: border-box;  
  .heading {
    max-width: 500px;
    margin: 0;
    line-height: 56px;
    font-weight: 700;
  }
`;

const StyledHeader = styled.div`
  display: flex;
  align-items: center;
  border-bottom: 1px solid #dee2e6;
`;

const CloseButton = styled(Icons.CrossSidebarIcon)`
cursor: pointer;
position: absolute;

width: 17px;
height: 17px;
min-width: 17px;
min-height: 17px;

right: 16px;
top: 19px;

  &:hover {
      path {
        fill: #657077;
      }
  }
`;

const Body = styled.div`
  position: relative;
  padding: 16px 0;
`;

const Footer = styled.div``;

class ModalDialog extends React.Component {
  constructor(props) {
    super(props);

    this.state = { displayType: this.getTypeByWidth() };

    this.getTypeByWidth = this.getTypeByWidth.bind(this);
    this.resize = this.resize.bind(this);
    this.popstate = this.popstate.bind(this);
    this.throttledResize = throttle(this.resize, 300);
  }

  getTypeByWidth() {
    if (this.props.displayType !== "auto") return this.props.displayType;

    return window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "modal";
  }

  resize() {
    if (this.props.displayType !== "auto") return;

    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;

    this.setState({ displayType: type });
  }

  popstate() {
    window.removeEventListener("popstate", this.popstate, false);
    this.props.onClose();
    window.history.go(1);
  }

  componentDidUpdate(prevProps) {
    if (this.props.displayType !== prevProps.displayType) {
      this.setState({ displayType: this.getTypeByWidth() });
    }
    if (this.props.visible && this.state.displayType === "aside") {
      window.addEventListener("popstate", this.popstate, false);
    }
  }

  componentDidMount() {
    window.addEventListener("resize", this.throttledResize);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  onKeyPress = event => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.props.onClose();
    }
  };

  render() {
    const {
      visible,
      scale,
      headerContent,
      bodyContent,
      footerContent,
      onClose,
      zIndex
    } = this.props;

    return this.state.displayType === "modal" ? (
      <Backdrop visible={visible} zIndex={zIndex}>
        <Dialog>
          <Content>
            <StyledHeader>
              <Heading
                className='heading'
                size='medium'
                truncate={true}
              >
                {headerContent}
              </Heading>
              <CloseButton onClick={onClose}></CloseButton>
            </StyledHeader>
            <Body>{bodyContent}</Body>
            <Footer>{footerContent}</Footer>
          </Content>
        </Dialog>
      </Backdrop>
    ) : (
        <div
          className={this.props.className}
          id={this.props.id}
          style={this.props.style}
        >
          <Backdrop visible={visible} onClick={onClose} zIndex={zIndex} />
          <Aside visible={visible} scale={scale} zIndex={zIndex} className="modal-dialog-aside">
            <Content>
              <StyledHeader>
                <Heading
                  className='heading'
                  size='medium'
                  truncate={true}
                >
                  {headerContent}
                </Heading>
                {scale ? <CloseButton onClick={onClose}></CloseButton> : ""}
              </StyledHeader>
              <Body>{bodyContent}</Body>
              <Footer className="modal-dialog-aside-footer">{footerContent}</Footer>
            </Content>
          </Aside>
        </div>
      );
  }
}

ModalDialog.propTypes = {
  visible: PropTypes.bool,
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  scale: PropTypes.bool,
  headerContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  bodyContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  footerContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  onClose: PropTypes.func,
  zIndex: PropTypes.number,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

ModalDialog.defaultProps = {
  displayType: "auto",
  zIndex: 310
};

export default ModalDialog;
