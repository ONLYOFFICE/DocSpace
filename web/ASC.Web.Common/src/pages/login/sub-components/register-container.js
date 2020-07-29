import React, { useState } from "react";
import { Box, Text, toastr } from "asc-web-components";
import RegisterModalDialog from "./register-modal-dialog";
import styled from "styled-components";
import PropTypes from 'prop-types';
import { sendRegisterRequest } from "../../../api/settings/index";

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
    const [loading, setLoading] = useState(false);

    const [email, setEmail] = useState("");
    const [emailErr, setEmailErr] = useState(false);

    const onRegisterClick = () => {
        setVisible(true);
    };
    const onRegisterModalClose = () => {
        setVisible(false);
        setEmail("");
        setEmailErr(false);
    };
    const onChangeEmail = (e) => {
        setEmail(e.currentTarget.value);
        setEmailErr(false);
    };
    const onSendRegisterRequest = () => {
        if (!email.trim()) {
            setEmailErr(true);
        }
        else {
            setLoading(true);
            sendRegisterRequest(email)
                .then(() => {
                    setLoading(false);
                    toastr.success("Successfully sent")
                })
                .catch((error) => {
                    setLoading(false);
                    toastr.error(error)
                })
                .finally(onRegisterModalClose)
        }
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
                    visible={visible}
                    loading={loading}
                    email={email}
                    emailErr={emailErr}
                    t={t}
                    onChangeEmail={onChangeEmail}
                    onRegisterModalClose={onRegisterModalClose}
                    onSendRegisterRequest={onSendRegisterRequest}
                />}
        </>
    )
}

Register.propTypes = {
    t: PropTypes.func.isRequired
};

export default Register;