import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'

import Avatar from '../avatar'
import Checkbox from '../checkbox'
import ContextMenuButton from '../context-menu-button'

const StyledContentRow = styled.div`
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
`;

const StyledContent = styled.div`
    display: flex;
    flex-basis: 100%;

    min-width: 160px;

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
`;

const StyledCheckbox = styled.div`
    flex-basis: 16px;
    display: flex;
`;

const StyledAvatar = styled.div`
    flex: 0 0 32px;
    display: flex;
    margin-left: 8px;
    margin-right: 8px;
    user-select: none;
`;

const StyledOptionButton = styled.div`
    flex: 0 0 16px;
    display: flex;
    margin-left: 8px;
    margin-right: 16px;
`;

class ContentRow extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      checked: this.props.checked
    }

    this.handleContextMenu = this.handleContextMenu.bind(this);
    this.changeCheckbox = this.changeCheckbox.bind(this);
    this.getOptions = this.getOptions.bind(this);
  };

  componentDidMount() {
    document.addEventListener('contextmenu', this.handleContextMenu);
  };

  componentWillUnmount() {
    document.removeEventListener('contextmenu', this.handleContextMenu);
  };

  handleContextMenu = (e) => {

  }

  changeCheckbox = (e) => {
    this.props.onSelect && this.props.onSelect(e.target.checked, this.props.data);
  };

  componentDidUpdate(prevProps, prevState) {
    if (this.props.checked !== prevProps.checked) {
      /*console.log(`ContentRow componentDidUpdate 
      this.props.checked=${this.props.checked}
      prevProps.checked=${prevProps.checked}
      this.state.checked=${this.state.checked}
      prevState.checked=${prevState.checked}`);*/

      this.setState({ checked: this.props.checked });
    }
  };

  getOptions = () => this.props.contextOptions;

  render() {
    //console.log("ContentRow render");
    const { checked, avatarRole, avatarSource, avatarName, children, contextOptions } = this.props;


    return (
      <StyledContentRow {...this.props}>
        {this.props.hasOwnProperty("checked") &&
          <StyledCheckbox>
            <Checkbox isChecked={checked} onChange={this.changeCheckbox} />
          </StyledCheckbox>
        }
        {(avatarRole !== '' || avatarSource !== '' || avatarName !== '') &&
          <StyledAvatar>
            <Avatar size='small' role={avatarRole || ''} source={avatarSource || ''} userName={avatarName || ''} />
          </StyledAvatar>
        }
        <StyledContent>{children}</StyledContent>
        <StyledOptionButton>
          {this.props.hasOwnProperty("contextOptions") && contextOptions.length > 0 &&
            <ContextMenuButton directionX='right' getData={this.getOptions} />
          }
        </StyledOptionButton>
      </StyledContentRow>
    );
  };
}

ContentRow.propTypes = {
  onSelect: PropTypes.func,
  avatarRole: PropTypes.string,
  avatarSource: PropTypes.string,
  avatarName: PropTypes.string,
  contextOptions: PropTypes.array,
  data: PropTypes.object,
  children: PropTypes.element
};

ContentRow.defaultProps = {
  avatarRole: '',
  avatarSource: '',
  avatarName: ''
};

export default ContentRow;