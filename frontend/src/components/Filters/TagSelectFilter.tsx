import React, { useCallback, useEffect, useState } from 'react'
import { Select } from 'antd'
import debounce from 'lodash.debounce'
import { IOptionsClient, OptionDto, OptionsClient } from '../../api/apiClient'

interface TagSelectFilterProps {
  selectedTags: OptionDto[]
  setSelectedTags: (tags: OptionDto[]) => void
  userId?: number // optional userId prop
}

const TagSelectFilter: React.FC<TagSelectFilterProps> = ({
  selectedTags,
  setSelectedTags,
  userId
}) => {
  const [options, setOptions] = useState<OptionDto[]>([])
  const [searchText, setSearchText] = useState<string>('')
  const [dropdownOpen, setDropdownOpen] = useState(false)
  const optionsClient: IOptionsClient = new OptionsClient()

  const fetchTags = useCallback(
    debounce(async (query: string) => {
      try {
        const result = await optionsClient.getTagOptions(
          1,
          20,
          userId !== undefined ? userId : undefined,
          query
        )
        const list = Array.isArray(result) ? result : (result?.items ?? [])
        setOptions(list)
      } catch (error) {
        console.error('Error fetching tag options:', error)
        setOptions([])
      }
    }, 500),
    [userId]
  )

  useEffect(() => {
    if (dropdownOpen) {
      fetchTags(searchText)
    }
  }, [searchText, dropdownOpen, fetchTags])

  const handleChange = (values: string[]) => {
    const selected = options.filter((opt) => values.includes(opt.value ?? ''))
    setSelectedTags(selected)
  }

  return (
    <Select
      mode="multiple"
      placeholder="Filter by tag"
      value={selectedTags.map((t) => t.value).filter((v): v is string => typeof v === 'string')}
      onChange={handleChange}
      onSearch={(text) => setSearchText(text)}
      onDropdownVisibleChange={(open) => {
        setDropdownOpen(open)
        if (!open) {
          setSearchText('')
          setOptions([])
        } else {
          fetchTags('')
        }
      }}
      filterOption={false}
      showSearch
      options={options.map((o) => ({
        label: o.displayText,
        value: o.value
      }))}
      style={{ width: '100%' }}
    />
  )
}

export default TagSelectFilter
