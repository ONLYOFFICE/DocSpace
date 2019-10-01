import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components'
import Backdrop from '../backdrop'
import { Text } from '../text'
import Aside from "../layout/sub-components/aside";

const Content = styled.div`
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

const SidePanel = props => {
  //console.log("SidePanel render");
  const { visible, scale, headerContent, bodyContent, footerContent, onClose } = props;

  return (
    <>
      <Backdrop visible={visible} onClick={onClose}/>
      <Aside visible={visible} scale={scale}>
        <Content>
          <Header>
            <HeaderText>{headerContent}</HeaderText>
            <CloseButton onClick={onClose}></CloseButton>
          </Header>
          <Body>{bodyContent}</Body>
          <Footer>{footerContent}</Footer>
        </Content>
      </Aside>
    </>
  );
};

SidePanel.propTypes = {
  visible: PropTypes.bool,
  scale: PropTypes.bool,
  headerContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  bodyContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  footerContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  onClose: PropTypes.func
}

export default SidePanel
