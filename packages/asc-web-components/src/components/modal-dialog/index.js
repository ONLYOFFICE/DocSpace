import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Backdrop from "../backdrop";
import Aside from "../aside";
import Heading from "../heading";
import { desktop } from "../../utils/device";
import throttle from "lodash/throttle";
import { Icons } from "../icons";
import Box from "../box";

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
  height: ${(props) => props.contentHeight};
  width: ${(props) => props.contentWidth};
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

const BodyBox = styled(Box)`
  position: relative;
`;

function Header() {
  return null;
}
Header.displayName = "DialogHeader";

function Body() {
  return null;
}
Body.displayName = "DialogBody";

function Footer() {
  return null;
}
Footer.displayName = "DialogFooter";

class ModalDialog extends React.Component {
  static Header = Header;
  static Body = Body;
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

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.props.onClose();
    }
  };

  render() {
    const {
      visible,
      scale,
      onClose,
      zIndex,
      bodyPadding,
      contentHeight,
      contentWidth,
      className,
      id,
      style,
      children,
    } = this.props;

    let header = null;
    let body = null;
    let footer = null;

    React.Children.forEach(children, (child) => {
      const childType =
        child && child.type && (child.type.displayName || child.type.name);

      switch (childType) {
        case Header.displayName:
          header = child;
          break;
        case Body.displayName:
          body = child;
          break;
        case Footer.displayName:
          footer = child;
          break;
        default:
          break;
      }
    });

    return this.state.displayType === "modal" ? (
      <Backdrop visible={visible} zIndex={zIndex}>
        <Dialog className={className} id={id} style={style}>
          <Content contentHeight={contentHeight} contentWidth={contentWidth}>
            <StyledHeader>
              <Heading className="heading" size="medium" truncate={true}>
                {header ? header.props.children : null}
              </Heading>
              <CloseButton onClick={onClose}></CloseButton>
            </StyledHeader>
            <BodyBox paddingProp={bodyPadding}>
              {body ? body.props.children : null}
            </BodyBox>
            <Box>{footer ? footer.props.children : null}</Box>
          </Content>
        </Dialog>
      </Backdrop>
    ) : (
      <Box className={className} id={id} style={style}>
        <Backdrop visible={visible} onClick={onClose} zIndex={zIndex} />
        <Aside
          visible={visible}
          scale={scale}
          zIndex={zIndex}
          className="modal-dialog-aside"
        >
          <Content contentHeight={contentHeight} contentWidth={contentWidth}>
            <StyledHeader className="modal-dialog-aside-header">
              <Heading className="heading" size="medium" truncate={true}>
                {header ? header.props.children : null}
              </Heading>
              {scale ? <CloseButton onClick={onClose}></CloseButton> : ""}
            </StyledHeader>
            <BodyBox
              className="modal-dialog-aside-body"
              paddingProp={bodyPadding}
            >
              {body ? body.props.children : null}
            </BodyBox>
            <Box className="modal-dialog-aside-footer">
              {footer ? footer.props.children : null}
            </Box>
          </Content>
        </Aside>
      </Box>
    );
  }
}

ModalDialog.propTypes = {
  children: PropTypes.any,
  visible: PropTypes.bool,
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  scale: PropTypes.bool,
  onClose: PropTypes.func,
  zIndex: PropTypes.number,
  bodyPadding: PropTypes.string,
  contentHeight: PropTypes.string,
  contentWidth: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

ModalDialog.defaultProps = {
  displayType: "auto",
  zIndex: 310,
  bodyPadding: "16px 0",
  contentWidth: "100%",
};

ModalDialog.Header = Header;
ModalDialog.Body = Body;
ModalDialog.Footer = Footer;

export default ModalDialog;
