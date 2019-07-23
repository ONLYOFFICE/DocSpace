import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'

import Avatar from '../avatar'
import Checkbox from '../checkbox'
import ContextMenuButton from '../context-menu-button'

const StyledContentRow = styled.div`
    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 12px;
    line-height: 16px;
    color: #A3A9AE;
    
    cursor: default;

    min-height: 47px;
    width: 100%;
    border-bottom: 1px solid #ECEEF1;

    display: flex;
    flex-direction: row;
    flex-wrap: nowrap;

    justify-content: flex-start;
    align-items: center;
    align-content: center;
    }
`;

const StyledContent = styled.div`
    display: flex;
    flex-basis: 100%;

    & > a, p {
        margin-left: 16px;
    }

    &.no-gutters {
        margin-right: 0;
        margin-left: 0;

        overflow:auto;
        white-space:nowrap;
      
        > .col,
        > [class*="col-"] {
          padding-right: 0;
          padding-left: 0;
        }
      }
`;

const StyledCheckbox = styled.div`
    flex-basis: 16px;
    display: flex;
    margin-right: 16px;
`;

const StyledAvatar = styled.div`
    flex: 0 0 32px;
    display: flex;
`;

const StyledOptionButton = styled.div`
    flex: 0 0 auto;
    display: flex;
    margin-left: 16px;
    margin-right: 16px;
`;

class ContentRow extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            checked: this.props.checked
        }
    }

    componentDidMount() {
        document.addEventListener('contextmenu', this.handleContextMenu);
    };

    componentWillUnmount() {
      document.removeEventListener('contextmenu', this.handleContextMenu);
    }

    handleContextMenu = (event) => {

    }

    componentDidUpdate(prevProps, prevState) {
        if(this.props.checked !== prevProps.checked) {
            /*console.log(`ContentRow componentDidUpdate 
            this.props.checked=${this.props.checked}
            prevProps.checked=${prevProps.checked}
            this.state.checked=${this.state.checked}
            prevState.checked=${prevState.checked}`);*/

            this.setState({checked: this.props.checked});
        }
    };

    render () {
        const { onSelect, avatarRole, avatarSource, avatarName, contextOptions, data, children } = this.props;
        

        return (
            <StyledContentRow {...this.props}>
                {this.props.hasOwnProperty("checked") && 
                    <StyledCheckbox>
                        <Checkbox isChecked={this.state.checked} onChange={(e) => { 
                            let checked = e.target.checked;
                            // console.log("ContentRow Checkbox onChange checked=", checked);
                            this.setState({checked: checked});
                            
                            if(onSelect) { 
                                onSelect(checked, data); 
                            }
                        }} />
                    </StyledCheckbox>
                }
                {(avatarRole !== '' || avatarSource !== '' || avatarName !== '')  && 
                    <StyledAvatar>
                        <Avatar size='small' role={avatarRole || ''} source={avatarSource || ''} userName={avatarName || ''} />
                    </StyledAvatar>
                }
                <StyledContent>{children}</StyledContent>
                {this.props.hasOwnProperty("contextOptions") && 
                    <StyledOptionButton>
                        <ContextMenuButton directionX='right' getData={() => contextOptions} />
                    </StyledOptionButton>
                }
            </StyledContentRow>
        );
    };
}

ContentRow.propTypes = {
};

ContentRow.defaultProps = {
};

export default ContentRow;