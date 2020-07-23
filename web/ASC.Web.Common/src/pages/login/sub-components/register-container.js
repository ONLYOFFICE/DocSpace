import React, { useState } from "react";
import { Box, Text } from "asc-web-components";
import RegisterModalDialog from "./register-modal-dialog";
import styled from "styled-components";
import PropTypes from 'prop-types';

const StyledRegister = styled(Box)`
  position: absolute;
  z-index: 184;
  width: 100%;
  height: 66px;
  padding: 1.5em;
  bottom: 0;
  right: 0;
  background-color: #F8F9F9;
  cursor: pointer;
`;

const Register = ({ t }) => {

    const [visible, setVisible] = useState(false);

    const onRegisterClick = () => {
        setVisible(true);
    };
    const onRegisterModalClose = () => {
        setVisible(false);
    };

    return (
        <>
            <StyledRegister onClick={onRegisterClick}>
                <Text color="#316DAA" textAlign="center">
                    {t("Register")}
                </Text>
            </StyledRegister>
            
            {visible &&
                <RegisterModalDialog
                    t={t}
                    visible={visible}
                    onRegisterModalClose={onRegisterModalClose} />}
        </>
    )
}

Register.propTypes = {
    t: PropTypes.func.isRequired
};

export default Register;