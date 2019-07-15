import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components'
import Backdrop from '../backdrop'

const Header = styled.div`
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  padding: 1rem;
  border-bottom: 1px solid #dee2e6;
  border-top-left-radius: .3rem;
  border-top-right-radius: .3rem;
`;

const HeaderTitle = styled.div`
  font-size: 1.5rem;
`;

const CloseButton = styled.button`
  background-color: transparent;
  border: 0;
  padding: 1rem;
  margin: -1rem -1rem -1rem auto;
  font-size: 1.5rem;
  font-weight: 700;
  line-height: 1;

  &:focus {
    outline: none;
  }
`;

const Body = styled.div`
  position: relative;
  flex: 1 1 auto;
  padding: 1rem;
`;

const Footer = styled.div`
  display: flex;
  align-items: center;
  justify-content: flex-start;
  padding: 1rem;
  border-top: 1px solid #dee2e6;
  border-bottom-right-radius: .3rem;
  border-bottom-left-radius: .3rem;
`;

const Content = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  width: 100%;
  pointer-events: auto;
  background-color: #fff;
  background-clip: padding-box;
  border: 1px solid rgba(0,0,0,.2);
  border-radius: .3rem;
  outline: 0;
`;

const Dialog = styled.div`
  position: relative;
  width: auto;
  max-width: 500px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  min-height: 100%;
`;

const ModalDialog = props => {
  const { visible, headerContent, bodyContent, footerContent, onClose } = props;

  return (
    <>
      <Backdrop visible={visible}>
        <Dialog>
          <Content>
            <Header>
              <HeaderTitle>{headerContent}</HeaderTitle>
              <CloseButton onClick={onClose}>×</CloseButton>
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
  onClose: PropTypes.func
}

export default ModalDialog
