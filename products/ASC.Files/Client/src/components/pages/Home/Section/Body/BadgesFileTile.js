import React from "react";
import styled from "styled-components";
import { Icons, Badge } from "asc-web-components";
import { canWebEdit, canConvert } from '../../../../../store/files/selectors';

const StyledBadgesFileTile = styled.div`
    display: flex;
    align-self: start;
    position: relative;
`;

class BadgesFileTile extends React.PureComponent  {

    render() {

        const { item } = this.props;
        const {
          fileExst,
          fileStatus,
          id,
          versionGroup
        } = item;
    
        const canEditFile = fileExst && canWebEdit(fileExst);
        const canConvertFile = fileExst && canConvert(fileExst);

        return(
            <StyledBadgesFileTile>
                {canConvertFile &&
                    <Icons.FileActionsConvertIcon
                        className='badge'
                        size='small'
                        isfill={true}
                        color='#A3A9AE'
                    />
                    }
                    {canEditFile &&
                    <Icons.AccessEditIcon
                        className='badge'
                        size='small'
                        isfill={true}
                        color='#A3A9AE'
                    />
                    }
                    {fileStatus === 1 &&
                    <Icons.FileActionsConvertEditDocIcon
                        className='badge'
                        size='small'
                        isfill={true}
                        color='#3B72A7'
                    />
                    }
                    {false &&
                    <Icons.FileActionsLockedIcon
                        className='badge'
                        size='small'
                        isfill={true}
                        color='#3B72A7'
                    />
                    }
                    {versionGroup > 1 &&
                    <Badge
                        className='badge-version'
                        backgroundColor="#A3A9AE"
                        borderRadius="11px"
                        color="#FFFFFF"
                        fontSize="10px"
                        fontWeight={800}
                        label={`Ver.${versionGroup}`}
                        maxWidth="50px"
                        onClick={this.onShowVersionHistory}
                        padding="0 5px"
                        data-id={id}
                    />
                    }
                    {fileStatus === 2 &&
                    <Badge
                        className='badge-version'
                        backgroundColor="#ED7309"
                        borderRadius="11px"
                        color="#FFFFFF"
                        fontSize="10px"
                        fontWeight={800}
                        label={`New`}
                        maxWidth="50px"
                        onClick={this.onBadgeClick}
                        padding="0 5px"
                        data-id={id}
                    />
                    }
            </StyledBadgesFileTile>
        )
    };
}

export default BadgesFileTile