import React from "react";
import { Box, Text } from "asc-web-components";

const ConsumerItem = (props) => {
    return (
        <>
            <Box displayProp="flex" flexDirection="column" marginProp="16px">
                <Box displayProp="flex" justifyContent="space-between" widthProp="100%">
                    <Box displayProp="flex">
                        <Text>
                            {props.name} logo
                        </Text>
                    </Box>
                    <Box>
                        toggle
                        </Box>
                </Box>
                <Box displayProp="flex" marginProp="10px 10px 10px auto">
                    <Text>
                        {props.description}
                    </Text>
                </Box>
            </Box>
        </>
    )
}

export default ConsumerItem;