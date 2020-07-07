import React, { useState } from "react";
import { Box, Text, Icons } from "asc-web-components";
import SubModalDialog from "./recover-modal-dialog";
import styled from "styled-components";
import PropTypes from 'prop-types';

const RecoverAccess = ({ t }) => {

    const RecoverContainer = styled(Box)`
    cursor: pointer;
    `;

    const [visible, setVisible] = useState(false);

    const onRecoverClick = () => {
        setVisible(true);
    }
    const onRecoverModalClose = () => {
        setVisible(false);
    }

    return (
        <>
            <Box widthProp="100%"
                heightProp="100%"
                paddingProp="0 240px 0 0"
                displayProp="flex"
                justifyContent="flex-end"
                alignItems="center">
                <RecoverContainer
                    backgroundProp="#27537F"
                    heightProp="100%"
                    displayProp="flex"
                    onClick={onRecoverClick}>
                    <Box paddingProp="16px 8px 16px 16px">
                        <Icons.UnionIcon />
                    </Box>
                    <Box paddingProp="18px 16px 18px 0px">
                        <Text color="#fff" isBold={true}>
                            {t("RecoverAccess")}
                        </Text>
                    </Box>
                </RecoverContainer>
            </Box>
            {visible && <SubModalDialog
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
