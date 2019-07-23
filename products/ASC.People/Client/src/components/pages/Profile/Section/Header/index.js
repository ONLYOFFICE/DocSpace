import React from 'react';
import { Text, IconButton } from 'asc-web-components';

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px"
};

const SectionHeaderContent = (props) => {
    const {profile, history} = props;

    return(
        <div style={wrapperStyle}>
            <IconButton iconName={'ArrowPathIcon'} color="#A3A9AE" size="16" onClick={history.goBack}/>
            <Text.ContentHeader style={textStyle}>{profile.displayName}</Text.ContentHeader>
        </div>
    );
};

export default SectionHeaderContent;