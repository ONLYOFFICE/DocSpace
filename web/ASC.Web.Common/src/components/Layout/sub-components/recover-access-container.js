import React, { useState } from "react";
import { Box, Text, Icons } from "asc-web-components";
import RecoverAccessModalDialog from "./recover-access-modal-dialog";
import styled from "styled-components";
import PropTypes from 'prop-types';

const RecoverContainer = styled(Box)`
 cursor: pointer;
 
  .recover-icon {
   @media(max-width: 375px) {
    padding: 16px;
   }
}
  .recover-text {
   @media(max-width: 375px) {
    display: none;
   }
}
`;

const RecoverAccess = ({ t }) => {

    const [visible, setVisible] = useState(false);

    const onRecoverClick = () => {
        setVisible(true);
    };
    const onRecoverModalClose = () => {
        setVisible(false);
    };

    return (
        <>
            <Box
                widthProp="100%"
                heightProp="100%"
                displayProp="flex"
                justifyContent="flex-end"
                alignItems="center">
                <RecoverContainer
                    backgroundProp="#27537F"
                    heightProp="100%"
                    displayProp="flex"
                    onClick={onRecoverClick}>
                    <Box paddingProp="16px 8px 16px 16px" className="recover-icon">
                        <Icons.UnionIcon />
                    </Box>
                    <Box paddingProp="18px 16px 18px 0px" className="recover-text" widthProp="100%">
                        <Text color="#fff" isBold={true}>
                            {t("RecoverAccess")}
                        </Text>
                    </Box>
                </RecoverContainer>
            </Box>
            {visible && <RecoverAccessModalDialog
                visible={visible}
                onRecoverModalClose={onRecoverModalClose}
                t={t}
            />
            }
        </>
    )
}

RecoverAccess.propTypes = {
    t: PropTypes.func.isRequired
};

export default RecoverAccess;
