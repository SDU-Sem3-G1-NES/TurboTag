import React, { useCallback, useEffect, useState } from 'react'
import type { SelectProps } from 'antd'
import { Select } from 'antd'
import debounce from 'lodash.debounce'
import { IOptionsClient, OptionDto, OptionsClient } from '../../api/apiClient'

interface UploaderSelectFilterProps {
  selectedUploaders: OptionDto[]
  setSelectedUploaders: (uploaders: OptionDto[]) => void
  userId?: number // optional userId prop
}

const UploaderSelectFilter: React.FC<UploaderSelectFilterProps> = ({
  selectedUploaders,
  setSelectedUploaders,
  userId
}) => {
  const [options, setOptions] = useState<OptionDto[]>([])
  const [searchText, setSearchText] = useState<string>('')
  const [dropdownOpen, setDropdownOpen] = useState(false)
  const optionsClient: IOptionsClient = new OptionsClient()

  const fetchUploaders = useCallback(
    debounce(async (query: string) => {
      try {
        const result = await optionsClient.getUploaderOptions(
          1,
          20,
          userId !== undefined ? userId : undefined,
          query
        )
        const list = Array.isArray(result)
          ? result
          : Array.isArray(result?.items)
            ? result.items
            : []

        setOptions(list)
      } catch (error) {
        console.error('Error fetching uploader options:', error)
        setOptions([])
      }
    }, 500),
    [userId]
  )

  useEffect(() => {
    if (dropdownOpen) {
      fetchUploaders(searchText)
    }
  }, [searchText, dropdownOpen, fetchUploaders])

  const handleChange: SelectProps['onChange'] = (value) => {
    const selectedOptions = Array.isArray(value) ? value : [value]
    const selected: OptionDto[] = selectedOptions.map(({ value, label }) => ({
      value,
      displayText: label
    }))
    setSelectedUploaders(selected)
  }

  return (
    <Select
      mode="multiple"
      placeholder="Filter by Uploader"
      labelInValue
      value={(selectedUploaders ?? []).map((t) => ({
        value: t.value ?? '',
        label: t.displayText ?? t.value ?? ''
      }))}
      onChange={handleChange}
      onSearch={(text) => setSearchText(text)}
      onDropdownVisibleChange={(open) => {
        setDropdownOpen(open)
        if (!open) {
          setSearchText('')
          setOptions([])
        } else {
          fetchUploaders('')
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

export default UploaderSelectFilter
