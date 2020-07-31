import React from 'react';
import PropTypes from "prop-types";
import styled from 'styled-components';

import { 
  ModalDialog, 
  EmailInput, 
  Button,
  utils 
} from 'asc-web-components';

const { tablet } = utils.device;

const Modal = ({ 
  t, 
  errorLoading, 
  visibleModal, 
  errorMessage, 
  emailOwner,
  settings,
  onEmailHandler,
  onSaveEmailHandler,
  onCloseModal 
}) => {

  let header, content, footer;

  const visible = errorLoading ? errorLoading : visibleModal;

  if(errorLoading) {
    header = t('errorLicenseTitle');
    content = <span 
      className="modal-error-content">
        {errorMessage ? errorMessage: t('errorLicenseBody')}
    </span>;

    footer = <Button
    className="modal-button-save"
    key="saveBtn"
    label={t('closeModalButton')}
    primary={true}
    size="medium"
    onClick={onCloseModal}
    />;

  } else if( visibleModal ) {
    header = t('changeEmailTitle');

    content = <EmailInput
      className="modal-change-email"
      tabIndex={1}
      id="change-email"
      name="email-wizard"
      placeholder={t('placeholderEmail')}
      emailSettings={settings}
      value={emailOwner}
      onValidateInput={onEmailHandler}
    />;

    footer = <Button
      className="modal-button-save"
      key="saveBtn"
      label={t('changeEmailBtn')}
      primary={true}
      size="medium"
      onClick={onSaveEmailHandler}
    />;
  }

  return <ModalDialog
      visible={visible}
      scale={false}
      displayType="auto"
      zIndex={310}
      headerContent={header}
      bodyContent={content}
      footerContent={footer}
      onClose={onCloseModal}
    />
}

const ModalContainer = styled(Modal)`
  .modal-change-email {
    height: 32px;
    width: 528px;

    @media ${tablet} {
      width: 293px;
    }
  }
  
  .modal-button-save {
    height: 36px;
    width: 100px;
    @media ${tablet} {
      width: 293px;
      height: 32px;
    }
  }
  
  .modal-error-content {
    font-size: 13px;
    line-height: 20px;
  }
`;

ModalContainer.propTypes = {
  t: PropTypes.func.isRequired,
  errorLoading: PropTypes.bool.isRequired,
  visibleModal: PropTypes.bool.isRequired,
  emailOwner: PropTypes.string,
  settings: PropTypes.object.isRequired,
  onEmailHandler: PropTypes.func.isRequired,
  onSaveEmailHandler: PropTypes.func.isRequired,
  onCloseModal: PropTypes.func.isRequired
};

export default ModalContainer;