import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components'
import Backdrop from '../backdrop'
import { Text } from '../text'

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
`;

const Header = styled.div`
  display: flex;
  align-items: center;
  border-bottom: 1px solid #dee2e6;
`;

const HeaderText = styled(Text.ContentHeader)`
  max-width: 500px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
`;

const CloseButton = styled.a`
  cursor: pointer;
  position: absolute;
  right: 16px;
  top: 20px;
  width: 16px;
  height: 16px;

  &:before, &:after {
    position: absolute;
    left: 8px;
    content: ' ';
    height: 16px;
    width: 1px;
    background-color: #D8D8D8;
  }
  &:before {
    transform: rotate(45deg);
  }
  &:after {
    transform: rotate(-45deg);
  }
`;

const Body = styled.div`
  position: relative;
  padding: 16px 0;
`;

const Footer = styled.div``;

const ModalDialog = props => {
  //console.log("ModalDialog render");
  const { visible, headerContent, bodyContent, footerContent, onClose, zIndex } = props;

  return (
    <>
      <Backdrop visible={visible} zIndex={zIndex} >
        <Dialog>
          <Content>
            <Header>
              <HeaderText>{headerContent}</HeaderText>
              <CloseButton onClick={onClose}></CloseButton>
            </Header>
            <Body>{bodyContent}</Body>
            <Footer>{footerContent}</Footer>
          </Content>
        </Dialog>
      </Backdrop>
    </>
  );
};

ModalDialog.propTypes = {
  visible: PropTypes.bool,
  headerContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  bodyContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  footerContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  onClose: PropTypes.func,
  zIndex: PropTypes.number
}

ModalDialog.defaultProps = {
  zIndex: 310
}

export default ModalDialog
