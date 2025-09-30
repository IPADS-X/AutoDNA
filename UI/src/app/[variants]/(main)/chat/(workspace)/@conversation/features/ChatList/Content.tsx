'use client';

import React, { memo, useCallback } from 'react';

import { SkeletonList, VirtualizedList } from '@/features/Conversation';
import { useFetchMessages } from '@/hooks/useFetchMessages';
import { useChatStore } from '@/store/chat';
import { chatSelectors } from '@/store/chat/selectors';

import MainChatItem from './ChatItem';
import Welcome from './WelcomeChatItem';

import { get_chats } from '@/custom/routing';

import { useState, useEffect } from 'react';
import { global_var, eventEmitter, state_map } from '@/custom/routing';

interface ListProps {
  mobile?: boolean;
}

const Content = memo<ListProps>(({ mobile }) => {
  const [isCurrentChatLoaded] = useChatStore((s) => [chatSelectors.isCurrentChatLoaded(s)]);

  useFetchMessages();
  // const data = useChatStore(chatSelectors.mainDisplayChatIDs);

  const store = useChatStore(chatSelectors.mainDisplayChatIDs);
  let itemContent = useCallback(
    (index: number, id: string) => <MainChatItem id={id} index={index} />,
    [mobile],
  );

  const [globalVar, setGlobalVar] = useState(global_var);
  const [, forceUpdate] = useState(0); 
  
  useEffect(() => {
      // 当count变化时，这个函数会被执行
      
  
      const handleStateChange = (data: any) => {
        // console.log('global_var', globalVar);
        // console.log('data', data);
        setGlobalVar(data);
        forceUpdate((s) => s + 1);
        // console.log('global_var3', globalVar);
      };
  
      eventEmitter.subscribe('stateChange', handleStateChange);
      
      // 注意：如果useEffect中有设置定时器或其他需要清理的副作用，
      // 应该返回一个清理函数。在这个例子中，因为我们没有这类操作，所以不需要返回。
    }, [globalVar]);


  if (!globalVar.started) {
    return <Welcome />;
  }



  let data = state_map[globalVar.state][0][globalVar.index].map((_, index) => store[0]);

  // console.log(data);

  // const data = get_chats_index();



  // if (!isCurrentChatLoaded) return <SkeletonList mobile={mobile} />;

  if (data.length === 0) return <Welcome />;

  return <>
  <Welcome />
  <VirtualizedList dataSource={state_map[globalVar.state][0][globalVar.index].map((_, index) => store[0])} itemContent={itemContent} mobile={mobile} />;
  </>
  });

Content.displayName = 'ChatListRender';

export default Content;
